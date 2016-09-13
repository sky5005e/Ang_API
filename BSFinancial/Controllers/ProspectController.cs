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
    [RoutePrefix("api/prospect")]
    public class ProspectController : ApiController
    {
        private BSFinancialRepository _repo;
        public ProspectController(BSFinancialRepository repo)
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
                var prospects = _repo.GetProspects();
                return Ok(prospects);
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
                var prospect = _repo.GetProspect(id);
                return Ok(prospect);
            }
            return null;
        }

        [Authorize]
        [HttpPost]
        [Route("")]
        public HttpResponseMessage Post(Prospect prospect)
        {
            if (ModelState.IsValid)
            {
                if (_repo.InsertOrUpdateProposal(prospect))
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, prospect);
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
                var prospects = _repo.DeleteProspect(id);
                return Ok(prospects);
            }
            return null;
        }
    }
}
