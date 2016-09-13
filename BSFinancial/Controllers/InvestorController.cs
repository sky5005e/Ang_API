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
    [RoutePrefix("api/Investor")]
    public class InvestorController : ApiController
    {
        private BSFinancialRepository _repo;

        public InvestorController(BSFinancialRepository repo)
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
                var investors = _repo.GetinvestorsByCompany(u.CompanyId).ToList();
                return Ok(investors);
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
                var investor = _repo.GetInvestor(id);
                return Ok(investor);
            }

            return null;
        }

        [Authorize]
        [HttpPost]
        [Route("")]
        public HttpResponseMessage Post(Investor investor)
        {
            if (ModelState.IsValid)
            {
                var currUser = AccountModel.GetCurrentUser(_repo);

                if (currUser != null)
                {
                    investor.CompanyId = currUser.CompanyId;
                }


                if (_repo.InsertOrUpdateInvestor(investor))
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, investor);
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
                var investors = _repo.DeleteInvestor(id, u.CompanyId);
                return Ok(investors);
            }

            return null;
        }

        [Authorize]
        [HttpGet]
        [ActionName("remove")]
        public IHttpActionResult Remove(int id)
        {
            try
            {
                var fund = _repo.GetFundInvestorById(id);
                var investors = _repo.RemoveFundInvestor(id, fund.FundId);
                return Ok(investors);
            
            }
            catch(Exception ex )
            {

            }
                
            return null;
        }

    }
}
