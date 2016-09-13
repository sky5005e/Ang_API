using BSFinancial.Data;
using BSFinancial.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace BSFinancial.Controllers
{
    [RoutePrefix("api/property")]
    public class PropertyController : ApiController
    {
        private BSFinancialRepository _repo;
        public PropertyController(BSFinancialRepository repo)
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

                var properties = _repo.GetPropertiesByCompany(u.CompanyId).ToList();
                foreach (var property in properties)
                {
                    if (property.LoanId > 0)
                    {
                        var _loanPayment = _repo.GetPaymentsOfLoan(Convert.ToInt32(property.LoanId)).OrderByDescending(x => x.Id).ToList();
                        if (_loanPayment.Any())
                        {
                            property.LastPayment = _loanPayment.FirstOrDefault().PayDate;
                        }
                    }
                }

                return Ok(properties);
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
                var property = _repo.GetProperty(id);
                return Ok(property);
            }

            return null;
        }

        [Authorize]
        [HttpPost]
        [Route("")]
        public HttpResponseMessage Post(Property property)
        {
            if (ModelState.IsValid)
            {
                var currUser = AccountModel.GetCurrentUser(_repo);

                if (currUser != null)
                {
                    property.CompanyId = currUser.CompanyId;
                }


                if (_repo.InsertOrUpdateProperty(property))
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, property);
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
                var loans = _repo.DeleteProperty(id, u.CompanyId);
                return Ok(loans);
            }
            return null;
        }

        [Authorize]
        [HttpGet]
        [ActionName("remove")]
        public IHttpActionResult Remove(int id)
        {
            var u = AccountModel.GetCurrentUser(_repo);

            if (u != null)
            {
                var loans = _repo.RemoveProperty(id, u.CompanyId);
                return Ok(loans);
            }
            return null;
        }
        [Authorize]
        [HttpGet]
        [ActionName("nonassctloanproperties")]
        public IHttpActionResult getNonAssctLoanProperties(int? id)
        {
            var u = AccountModel.GetCurrentUser(_repo);

            if (u != null)
            {
                var properties = _repo.GetPropertiesByCompany(u.CompanyId).Where(q => q.LoanId == null).Select(s => new { id = s.Id, address = s.Address, city = s.City }).ToList();
                return Ok(properties);
            }

            return null;
        }

        [Authorize]
        [HttpPost]
        [ActionName("uploadDocument")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage uploadDocument()
        {
            var u = AccountModel.GetCurrentUser(_repo);

            if (u != null)
            {
                var httpRequest = HttpContext.Current.Request;
                var postedFile = httpRequest.Files[0];
                int propertyID = Convert.ToInt32(httpRequest.Form["propertyId"]);
                String description = Convert.ToString(httpRequest.Form["description"]);
                string uploadFile = "/upload/" + Guid.NewGuid().ToString() + "-" + postedFile.FileName; // you could put this to web.config

                string fullFilePath = HttpContext.Current.Server.MapPath(uploadFile);
                PropertyDocument doc = new PropertyDocument();
                doc.description = description;
                doc.CreatedOn = DateTime.Now;
                doc.propertyId = propertyID;
                doc.filePath = uploadFile;
                if (_repo.InsertPropertyDocument(doc))
                {
                    postedFile.SaveAs(fullFilePath);
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, doc);
                    return response;
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

            }
            return null;
        }

        [Authorize]
        [HttpPost]
        [ActionName("deleteDocument")]
        public IHttpActionResult deleteDocument(int id, [FromBody]ParentChild model)
        {
            var u = AccountModel.GetCurrentUser(_repo);

            if (u != null)
            {
                var documents = _repo.DeletePropertyDocument(id, model.ChildId);
                return Ok(documents);
            }

            return null;
        }

        [Authorize]
        [HttpPost]
        [ActionName("uploadPhoto")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage uploadPhoto()
        {
            var u = AccountModel.GetCurrentUser(_repo);

            if (u != null)
            {
                var httpRequest = HttpContext.Current.Request;
                var postedFile = httpRequest.Files[0];
                int propertyID = Convert.ToInt32(httpRequest.Form["propertyId"]);
                string description = Convert.ToString(httpRequest.Form["description"]);
                string uploadFile = "/photos/" + Guid.NewGuid().ToString() + "-" + postedFile.FileName; // you could put this to web.config

                string fullFilePath = HttpContext.Current.Server.MapPath(uploadFile);
                PropertyPhoto photo = new PropertyPhoto();
                photo.description = description;
                photo.CreatedOn = DateTime.Now;
                photo.propertyId = propertyID;
                photo.filePath = uploadFile;
                if (_repo.InsertPropertyPhoto(photo))
                {
                    postedFile.SaveAs(fullFilePath);
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, photo);
                    return response;
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
            }
            return null;
        }

    }
}
