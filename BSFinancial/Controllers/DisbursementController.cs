using BSFinancial.Data;
using BSFinancial.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BSFinancial.Controllers
{
    [RoutePrefix("api/disbursement")]
    public class DisbursementController : ApiController
    {
        private BSFinancialRepository _repo;

        public DisbursementController(BSFinancialRepository repo)
        {
            _repo = repo;
        }

        [Authorize]
        [Route("")]
        public IHttpActionResult Get(int loanID)
        {
            var u = AccountModel.GetCurrentUser(_repo);

            if (u != null)
            {
                var disbursements = _repo.GetDisbursementsOfLoan(loanID).ToList();
                return Ok(disbursements);
            }

            return null;
        }

        
        [Authorize]
        [HttpPost]
        [Route("")]
        public HttpResponseMessage Post(Disbursement disbursement)
        {
            if (ModelState.IsValid)
            {
                var currUser = AccountModel.GetCurrentUser(_repo);

                if (currUser != null)
                {
                    disbursement.CompanyId = currUser.CompanyId;
                }

                if (_repo.InsertOrUpdateDisbursement(disbursement))
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, disbursement);
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
                var rst = _repo.DeleteDisbursement(id, u.CompanyId);
                return Ok(rst);
            }

            return null;
        }
    }
}