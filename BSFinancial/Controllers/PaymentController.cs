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
    [RoutePrefix("api/payment")]
    public class PaymentController : ApiController
    {
        private BSFinancialRepository _repo;

        public PaymentController(BSFinancialRepository repo)
        {
            _repo = repo;
        }

        //// GET api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}
        [Authorize]
        [Route("")]
        public IHttpActionResult Get()
        {
            var u = AccountModel.GetCurrentUser(_repo);

            if (u != null)
            {
                var payments = _repo.GetAllLoansPaymentByCompany(u.CompanyId).ToList();
                return Ok(payments);
            }

            return null;
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        [Authorize]
        [HttpPost]
        [Route("")]
        public HttpResponseMessage Post(Payment payment)
        {
            if (ModelState.IsValid)
            {
                var currUser = AccountModel.GetCurrentUser(_repo);

                if (currUser != null)
                {
                    payment.CompanyId = currUser.CompanyId;
                }

                if (_repo.InsertOrUpdatePayment(payment))
                {
                    //var payments = _repo.GetPaymentsOfLoan(payment.LoanId);
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, payment);
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
                var rst = _repo.DeletePayment(id, u.CompanyId);
                return Ok(rst);
            }

            return null;
        }
    }
}