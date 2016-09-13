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
    [RoutePrefix("api/applicant")]
    public class ApplicantController : ApiController
    {
        private BSFinancialRepository _repo;

        public ApplicantController(BSFinancialRepository repo)
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
                var applicants = _repo.GetApplicantsByCompany(u.CompanyId).ToList();
                return Ok(applicants);
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
                var applicant = _repo.GetApplicant(id);
                return Ok(applicant);
            }

            return null;
        }

        [HttpPost]
        [Route("")]
        public HttpResponseMessage Post(Applicant applicant)
        {
            if (ModelState.IsValid)
            {
                var currUser = AccountModel.GetCurrentUser(_repo);

                if (currUser != null)
                {
                    applicant.CompanyId = currUser.CompanyId;
                }

                if (_repo.InsertOrUpdateApplicant(applicant))
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, applicant);
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
                var applicants = _repo.DeleteApplicant(id, u.CompanyId);
                return Ok(applicants);
            }

            return null;
        }
    }
}