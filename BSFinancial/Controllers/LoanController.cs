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
    [RoutePrefix("api/loan")]
    public class LoanController : ApiController
    {
        private BSFinancialRepository _repo;

        public LoanController(BSFinancialRepository repo)
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
                try
                {
                    var loans = _repo.GetLoansByCompany(u.CompanyId).ToList();
                    for (int i = 0; i < loans.Count; i++)
                    {
                        loans[i].application = _repo.GetApplication(loans[i].ApplicationId);
                    }
                    return Ok(loans);
                }
                catch (Exception ex)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        ReasonPhrase = "Get Loan: " + ex.Message
                    });
                }
            }
            else
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    ReasonPhrase = "Get Loan: Cannot find user"
                });
            }
        }

        [Authorize]
        [Route("{id}")]
        public IHttpActionResult Get(int id)
        {
            var u = AccountModel.GetCurrentUser(_repo);

            if (u != null)
            {
                var loan = _repo.GetLoan(id);
                return Ok(loan);
            }

            return null;
        }
       
        [Authorize]
        [HttpPost]
        [Route("")]
        public HttpResponseMessage Post(Loan loan)
        {
            if (ModelState.IsValid)
            {
                var currUser = AccountModel.GetCurrentUser(_repo);

                if (currUser != null)
                {
                    loan.CompanyId = currUser.CompanyId;
                }

                if (loan.application == null)
                {
                    ModelState.AddModelError("emptyApplication", "Please select application");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

                if (_repo.InsertOrUpdateLoan(loan))
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, loan);
                    return response;
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
            }
            else
            {
                //return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    ReasonPhrase = "Update Report: Report Model State Invalid: " + message
                });
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
                var loans = _repo.DeleteLoan(id, u.CompanyId);
                return Ok(loans);
            }

            return null;
        }

        [Authorize]
        [HttpPost]
        [ActionName("savePayment")]
        //[Route("savePayment")]
        public HttpResponseMessage SavePayment(Payment payment)
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
        [HttpPost]
        [ActionName("saveLoanProperties")]
        public IHttpActionResult SaveLoanProperties(int id, [FromBody]LoanPropertyModel model)
        {
            var u = AccountModel.GetCurrentUser(_repo);

            if (u != null)
            {
                if (_repo.AddPropertiesToLoan(model.LoanId, model.Properties))
                {
                    // return result
                    var properties = _repo.GetPropertiesOfLoan(model.LoanId).ToList();
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
        [ActionName("uploadDocument")]
        [AcceptVerbs("POST")]
        public HttpResponseMessage uploadDocument()
        {
            var u = AccountModel.GetCurrentUser(_repo);

            if (u != null)
            {
                var httpRequest = HttpContext.Current.Request;
                var postedFile = httpRequest.Files[0];
                int loanID = Convert.ToInt32(httpRequest.Form["loanId"]);
                String description = Convert.ToString(httpRequest.Form["description"]);
                string uploadFile = "/upload/" + Guid.NewGuid().ToString() + "-" + postedFile.FileName; // you could put this to web.config
                
                string fullFilePath = HttpContext.Current.Server.MapPath(uploadFile);
                Document doc = new Document();
                doc.description = description;
                doc.CreatedOn = DateTime.Now;
                doc.LoanId = loanID;
                doc.CompanyId = u.CompanyId;
                doc.filePath = uploadFile;
                if (_repo.InsertDocument(doc))
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
        [HttpGet]
        [ActionName("deleteDocument")]
        public IHttpActionResult deleteDocument(int id)
        {
            var u = AccountModel.GetCurrentUser(_repo);

            if (u != null)
            {
                var documents = _repo.DeleteDocument(id, u.CompanyId);
                return Ok(documents);
            }

            return null;
        }

        [Authorize]
        [HttpGet]
        [ActionName("escrowbalance")]
        public IHttpActionResult EscrowBalance(int id)
        {
            var u = AccountModel.GetCurrentUser(_repo);

            if (u != null)
            {
                var loan = _repo.GetLoan(id);
                var escBal = loan.Payments.Sum(s => s.PrincipalAmt) - loan.Disbursements.Sum(s => s.Amount);
                return Ok(escBal);
            }

            return null;
        }
    }
}