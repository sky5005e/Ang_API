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
    [RoutePrefix("api/employment")]
    public class EmploymentController : ApiController
    {
        private BSFinancialRepository _repo;

        public EmploymentController(BSFinancialRepository repo)
        {
            _repo = repo;
        }

        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        [Authorize]
        [HttpPost]
        [Route("")]
        public HttpResponseMessage Post(Employment employment)
        {
            if (ModelState.IsValid)
            {
                var currUser = AccountModel.GetCurrentUser(_repo);

                if (currUser != null)
                {
                    employment.CompanyId = currUser.CompanyId;
                }

                if (_repo.InsertOrUpdateEmployment(employment))
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, employment);
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
                var rst = _repo.DeleteEmployment(id, u.CompanyId);
                return Ok(rst);
            }

            return null;
        }
    }
}