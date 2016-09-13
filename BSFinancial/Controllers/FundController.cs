using BSFinancial.Data;
using BSFinancial.Models;
using BSFinancial.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;

namespace BSFinancial.Controllers
{
    [RoutePrefix("api/Fund")]
    public class FundController : ApiController
    {
        private BSFinancialRepository _repo;

        public FundController(BSFinancialRepository repo)
        {
            _repo = repo;
        }

        [Authorize]
        [Route("")]
        public IHttpActionResult Get()
        {
            var u = AccountModel.GetCurrentUser(_repo);

            if (u != null)
            {
                var funds = _repo.GetFundsByCompany(u.CompanyId).ToList();
                return Ok(funds);
            }

            return null;
        }

        [Authorize]
        [Route("{id}")]
        public IHttpActionResult Get(int id)
        {
            var u = AccountModel.GetCurrentUser(_repo);

            if (u != null)
            {
                var fund = _repo.GetFund(id);
                return Ok(fund);
            }

            return null;
        }
       
        [Authorize]
        [HttpPost]
        [Route("")]
        public HttpResponseMessage Post(Fund fund)
        {
            if (ModelState.IsValid)
            {
                var currUser = AccountModel.GetCurrentUser(_repo);

                if (currUser != null)
                {
                    fund.CompanyId = currUser.CompanyId;
                }

                if (_repo.InsertOrUpdateFund(fund))
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, fund);
                    return response;
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        [Authorize]
        [HttpGet]
        [ActionName("delete")]
        public IHttpActionResult Delete(int id)
        {
            var u = AccountModel.GetCurrentUser(_repo);

            if (u != null)
            {
                var funds = _repo.DeleteFund(id, u.CompanyId);
                return Ok(funds);
            }

            return null;
        }

        [Authorize]
        [HttpPost]
        [ActionName("saveFundInvestors")]
        public IHttpActionResult SaveFundInvestors(int id, [FromBody]FundInvestorModel model)
        {
            var u = AccountModel.GetCurrentUser(_repo);

            if (u != null)
            {
                if (_repo.AddInvestorsToFund(model.FundId, model.Investors))
                {
                    // return result
                    var investors = _repo.GetInvestorsOfFund(model.FundId).ToList();
                    return Ok(investors);
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        [Authorize]
        [HttpPost]
        [ActionName("updateFundInvestors")]
        public IHttpActionResult UpdateFundInvestors(int id, [FromBody]FundInvModel model)
        {
            var u = AccountModel.GetCurrentUser(_repo);

            if (u != null)
            {

                if (_repo.UpdateInvestorsToFund(model.FundId, model.Investor))
                {
                    // return result
                    var investors = _repo.GetInvestorsOfFund(model.FundId).ToList();
                    return Ok(investors);
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        [Authorize]
        [HttpPost]
        [ActionName("saveFundDistribution")]
        public IHttpActionResult SaveFundDistribution(int id, [FromBody]DistributionModel distribution)
        {
            var u = AccountModel.GetCurrentUser(_repo);

            if (u != null)
            {

                if (_repo.AddDistributionToFund(distribution))
                {
                    // return result
                    var distributions = _repo.GetDistributionsOfFund(distribution.FundId).ToList();
                    return Ok(distributions);
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        [Authorize]
        [HttpGet]
        [ActionName("deleteInvestorDistribution")]
        public IHttpActionResult DeleteInvestorDistribution(int id)
        {
            try
            {
                var distribution = _repo.GetDistributionById(id);
                var distributions = _repo.DeleteInvestorDistributionById(id, distribution.FundId);
                return Ok(distributions);

            }
            catch (Exception ex)
            {

            }

            return null;
        }
    }
}
