using BSFinancial.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

using System.Threading.Tasks;
using System.Web.Http;
using BSFinancial.Services;
using BSFinancial.Models;

namespace BSFinancial.Controllers
{
    [RoutePrefix("api/inquiry")]
    public class InquiryController : ApiController
    {
        private BSFinancialRepository _repo;
        private IMailService _mail;

        public InquiryController(BSFinancialRepository repo)
        {
            _mail = new MailService();
            _repo = repo;
        }


        [Authorize]
        [Route("")]
        public IHttpActionResult Get()
        {
            var u = AccountModel.GetCurrentUser(_repo);

            if (u != null)
            {
                var inquries = _repo.GetInquiriesByCompany(u.CompanyId).ToList();
                return Ok(inquries);
            }

            return null;
        }

        [HttpPost]
        [Route("")]
        public HttpResponseMessage Post(ContactModel inquiry)
        {
            if (ModelState.IsValid)
            {
                var currUser = AccountModel.GetCurrentUser(_repo);

                if (currUser != null)
                {
                    inquiry.CompanyId = currUser.CompanyId;
                }

                var msg = string.Format("Comment From: {1}{0}Loan Type: {2}{0}Property Cost {3}{0}Email: {4}{0}Phone: {5}{0}Current Address: {6}{0}Rent/Own: {7}{0}" +
                            "Monthly Rent: {8}{0}Employed By: {9}{0}Monthly Income: {10}{0}Comments: {11}{0}",
                            Environment.NewLine, inquiry.Name, inquiry.LoanType, inquiry.PropertyCost, inquiry.Email, inquiry.Phone,
                            inquiry.CurrentAddress, inquiry.RentOwn, inquiry.MonthlyRent, inquiry.EmployedBy, inquiry.MonthlyIncome, inquiry.Comments);

                if (_mail.SendMail("jimmycai888@yahoo.com", "jimmycai888@yahoo.com", "From Contact Page", msg))
                {
                    if (_repo.AddInquiry(inquiry) && _repo.Save())
                    {
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, inquiry);
                        //response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = inquiry.Id }));
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
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

    }
}
