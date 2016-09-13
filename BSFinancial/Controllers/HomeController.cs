using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BSFinancial.Models;
using BSFinancial.Services;
using BSFinancial.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;


namespace BSFinancial.Controllers
{
    public class HomeController : Controller
    {
        private IMailService _mail;
        private IBSFinancialRepository _repo;
        private ApplicationUserManager userManager;
        public ApplicationUserManager UserManager
        {
            get { return userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { userManager = value; }
        }
        public HomeController(IMailService mail, IBSFinancialRepository repo)
        {
            _mail = mail;
            _repo = repo;
        }

        private async Task<User> GetCurrentUser()
        {
            string email = User.Identity.GetUserName();
            if (email != null && email != "")
            {
                //var email = userManager.FindByEmail()
                var user = _repo.GetUserByEmail(email);
                return user;

                //IdentityUser identity = await UserManager.FindByIdAsync(accountId);
                //if (identity == null)
                //{
                //    return null;
                //}
            }
            else return null;

            

            //var currentUser = await db.Users.SingleOrDefaultAsync(e => e.AccountId == accountId);
            //if (currentUser == null)
            //{
            //    return null;
            //}

            
            //return new UserService(db, currentUser);
        }
        public ActionResult Index()
        {
            //var u = GetCurrentUser().Result;

            //if (u != null)
            //{
            //    var companies = _repo.GetCompanies(u.CompanyId).ToList();
            //    return View(companies);
            //}
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ContactSucc()
        {

            return View();
        }
        public ActionResult MyMessages()
        {
            return View(); 
        }

        [Authorize]
        public ActionResult InquiryList()   
        {
            //var u = GetCurrentUser().Result;

            //if (u != null)
            //{
            //    var inquries = _repo.GetInquiriesByCompany(u.CompanyId).ToList();
            //    return View(inquries);
            //}
            return View();
        }
        public ActionResult InquiryDetail(int inquiryId) 
        {
            var u = GetCurrentUser().Result;

            if (u != null)
            {
                var inquiry = _repo.GetInquiry(inquiryId);
                return View(inquiry);
            }
            return View();
        }
        public ActionResult LoanApplication()   
        {
            return View();
        }
        public ActionResult SiteMap()   
        {
            return View();
        }
    }
}
