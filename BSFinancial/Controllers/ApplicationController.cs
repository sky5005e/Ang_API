using BSFinancial.Data;
using BSFinancial.Models;
using BSFinancial.Models.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BSFinancial.Controllers
{
    [EnableCors(origins: "http://localhost:3242", headers: "*", methods: "*", SupportsCredentials = true)]
    [RoutePrefix("api/application")]
    public class ApplicationController : ApiController
    {
        private BSFinancialRepository _repo;

        public ApplicationController(BSFinancialRepository repo)
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
                var applications = _repo.GetApplicationsByCompany(u.CompanyId).ToList();
                return Ok(applications);
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
                var application = _repo.GetApplication(id);
                return Ok(application);
            }

            return null;
        }

        [Authorize]
        [HttpPost]
        [Route("")]
        public HttpResponseMessage Post(Application application)
        {
            if (ModelState.IsValid)
            {
                var currUser = AccountModel.GetCurrentUser(_repo);

                if (currUser != null)
                {
                    application.CompanyId = currUser.CompanyId;
                }

//                 if (application.ApplicantId == 0)
//                 {
//                     ModelState.AddModelError("emptyApplicant", "Please select applicant");
//                     return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
//                 }

                if (_repo.InsertOrUpdateApplication(application))
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, application);
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
        [HttpPost]
        //[EnableCors(origins: "http://localhost:3242", headers: "*", methods: "get,post")]
        [ActionName("saveapplicants")]
        //[Route("saveapplicants/{app}")]

        public HttpResponseMessage SaveApplicants([FromBody]Application app)
        {
            if (ModelState.IsValid)
            {
                var currUser = AccountModel.GetCurrentUser(_repo);

                if (currUser != null)
                {
                    app.CompanyId = currUser.CompanyId;
                }

                if (_repo.SaveApplicants(app))
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, true);
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
                var applications = _repo.DeleteApplication(id, u.CompanyId);
                return Ok(applications);
            }

            return null;
        }

        [Authorize]
        [HttpPut]
        [ActionName("setMainApplicant")]
        [System.Web.Http.AcceptVerbs("PUT")]
        public IHttpActionResult setMainApplicant(int id, [FromBody]int applicantId)
        {
            var u = AccountModel.GetCurrentUser(_repo);

            if (u != null)
            {
                var rst = _repo.SetMainApplicant(id, applicantId);
                return Ok(rst);
            }

            return null;
        }

        [Authorize]
        [HttpPut]
        [ActionName("removeApplicant")]
        [System.Web.Http.AcceptVerbs("PUT")]
        public IHttpActionResult removeApplicant(int id, [FromBody]int applicantId)
        {
            var u = AccountModel.GetCurrentUser(_repo);

            if (u != null)
            {
                var rst = _repo.RemoveApplicant(id, applicantId);
                return Ok(rst);
            }

            return null;
        }
    }
}