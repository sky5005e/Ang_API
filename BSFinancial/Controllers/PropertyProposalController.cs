using BSFinancial.Data;
using BSFinancial.Models;
using BSFinancial.Providers;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace BSFinancial.Controllers
{
    [RoutePrefix("api/propertyproposal")]
    public class PropertyProposalController : ApiController
    {
        private BSFinancialRepository _repo;
        public PropertyProposalController(BSFinancialRepository repo)
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
                var prospects = _repo.GetProposalProspects();
                return Ok(prospects);
            }
            return null;
        }

        [AllowAnonymous]
        [Route("{id}")]
        public IHttpActionResult Get(int id)
        {
            // var u = AccountModel.GetCurrentUser(_repo);

            // if (u != null)
            //{
            var prospect = _repo.GetProposalProperty(id);
            if (prospect != null)
            {
                _repo.UpdateProposalProspects(id, DateTime.Now, null);
                return Ok(prospect);
            }
            return null;
        }

        [Authorize]
        [HttpPost]
        [Route("")]
        public HttpResponseMessage Post(Prospect proposal)
        {
            if (ModelState.IsValid)
            {
                if (_repo.InsertOrUpdateProposal(proposal))
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, proposal);
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
                var prospects = _repo.DeleteProposalProspect(id);
                return Ok(prospects);
            }
            return null;
        }

        [Authorize]
        [HttpGet]
        [ActionName("getProposal")]
        public IHttpActionResult GetProposal(int? id)
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
        [HttpPost]
        [ActionName("savePropertyProspect")]
        public IHttpActionResult SavePropertyProspect(int id, [FromBody]ProposalProspectModel model)
        {
            var u = AccountModel.GetCurrentUser(_repo);

            if (u != null && model.ProspectId > 0)
            {
                ProposalProspects p = new ProposalProspects();
                p.ProspectId = model.ProspectId;
                p.UserId = model.UserId;
                p.ProposalDate = DateTime.Now;
                p.ProposalFor = model.ProposalFor;
                p.TokenKey = EncryptionProviders.RandomString(32);
                if (_repo.AddProposalProspects(p))
                {
                    // return result
                    var prospects = _repo.GetProposalProspects();
                    return Ok(prospects);
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
        [ActionName("saveProposalProperties")]
        public IHttpActionResult SaveProposalProperties(int id, [FromBody]ProposalPropertyModel model)
        {
            var u = AccountModel.GetCurrentUser(_repo);

            if (u != null)
            {
                ProposalProperty p = new ProposalProperty();
                p.ProposalId = model.ProposalId;
                p.PropertyID = model.Properties[0].Id;
                p.CreatedDate = DateTime.Now;
                if (_repo.AddProposalProperties(p))
                {
                    // return result
                    var properties = _repo.GetProposalPropertiesInfo(model.ProposalId).ToList();
                    return Ok(properties);
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
        [ActionName("removeProposalProperty")]
        public IHttpActionResult removeProposalProperty(int id, [FromBody]ParentChild model)
        {
            var u = AccountModel.GetCurrentUser(_repo);

            if (u != null)
            {

                if (_repo.RemoveProposalProperty(model.Id, model.ChildId))
                {
                    // return result
                    var properties = _repo.GetProposalPropertiesInfo(model.Id).ToList();
                    return Ok(properties);
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
        [ActionName("sendEmail")]
        public async Task<IHttpActionResult> sendEmail(int id)
        {
            var u = AccountModel.GetCurrentUser(_repo);

            if (u != null)
            {
                var prospects = _repo.GetProposalProspects();
                var prospect = prospects.Where(p => p.Id == id).FirstOrDefault();
                _repo.UpdateProposalProspects(id, null, DateTime.Now);
                string Domain = HttpContext.Current.Request.Url.Scheme + System.Uri.SchemeDelimiter + HttpContext.Current.Request.Url.Host + (HttpContext.Current.Request.Url.IsDefaultPort ? "" : ":" + HttpContext.Current.Request.Url.Port);
                var callbackUrl = String.Format("{0}/#/proposalproperties/{1}/{2}", Domain, id, prospect.TokenKey);

                IdentityMessage message = new IdentityMessage();
                message.Destination = prospect.Email;
                message.Body = "You have been invited by (" + u.Email + ") to view proposals!!! <br/> Please view this inviation :  <a href=\"" + callbackUrl + "\">View Proposal Invitation </a>";
                message.Subject = "View Proposal Invitation!!!";
                await new EmailService().SendEmailAsync(message);

                return Ok(prospects);
            }
            return null;
        }

        [AllowAnonymous]
        [HttpGet]
        [ActionName("getPropertyViewDetails")]
        public IHttpActionResult getPropertyViewDetails(int id)
        {
            //var u = AccountModel.GetCurrentUser(_repo);

            //if (u != null)
            //{
            var properties = _repo.GetViewedProperty(id);
            return Ok(properties);
            //}
            // return null;
        }

        [AllowAnonymous]
        [HttpPost]
        [ActionName("savePropertyViewDetails")]
        public IHttpActionResult SavePropertyViewDetails(int id, [FromBody]ParentChild model)
        {
            // var u = AccountModel.GetCurrentUser(_repo);

            //if (u != null)
            //{
            PropertyViewInfo pvi = new PropertyViewInfo();
            // pvi.CompanyId = u.CompanyId;
            // pvi.UserId = u.Id;
            pvi.PropertyId = model.ChildId;
            pvi.ProposalId = model.Id;
            pvi.LastViewDate = DateTime.Now;
            pvi.ViewDuration = 1;
            if (_repo.InsertOrUpdatePropertyViewInfo(pvi))
            {
                // return result
                return Ok();
            }
            else
            {
                return null;
            }
            //}

            //return null;
        }

        [AllowAnonymous]
        [HttpGet]
        [ActionName("getPhotosbyPropoertyId")]
        public IHttpActionResult GetPhotosbyPropoertyId(int id)
        {

            if (id > 0)
            {
                var photos = _repo.GetPhotosOfProperty(id);
                return Ok(photos);
            }
            else
            {
                return null;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [ActionName("getPropertyRating")]
        public IHttpActionResult getPropertyRating(int id)
        {

            if (id > 0)
            {
                var rating = _repo.GetPropertyRating(id);
                return Ok(rating);
            }
            else
            {
                return null;
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [ActionName("saveRatingInfo")]
        public IHttpActionResult SaveRatingInfo(int id, [FromBody]PropertyRating model)
        {
            if (_repo.InsertOrUpdatePropertyRatingInfo(model))
            {
                // return result
                return Ok(model);
            }
            else
            {
                return null;
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [ActionName("savePropertyViewedTime")]
        public IHttpActionResult savePropertyViewedTime(int id, [FromBody]PropertyViewInfo model)
        {
            if (_repo.InsertOrUpdatePropertyViewInfo(model))
            {
                // return result
                return Ok(model);
            }
            else
            {
                return null;
            }
        }

    }
}
