using BSFinancial.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BSFinancial.Controllers
{
    public class CompaniesController : ApiController
    {
        private IBSFinancialRepository _repo;
        public CompaniesController(IBSFinancialRepository repo)
        {
            _repo = repo;
        }
        public IEnumerable<Company> Get()
        {
            var u = GetCurrentUser().Result;
            //string accountId = User.Identity.GetUserId();
            var companies = _repo.GetCompanies(u.CompanyId).ToList();
            return companies;
        }

        public HttpResponseMessage Post([FromBody]Company newCompany)
        {
            if (newCompany.CreatedOn == default(DateTime))
            {
                newCompany.CreatedOn = DateTime.UtcNow;
            }

            if (_repo.AddCompany(newCompany) &&
                    _repo.Save())
            {
                return Request.CreateResponse(HttpStatusCode.Created, newCompany);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        private async Task<User> GetCurrentUser()
        {
            string accountId = User.Identity.GetUserId();
            //IdentityUser identity = await UserManager.FindByIdAsync(accountId);

            //if (identity == null)
            //{
            //    return null;
            //}

            //var currentUser = await db.Users.SingleOrDefaultAsync(e => e.AccountId == accountId);
            //if (currentUser == null)
            //{
            //    return null;
            //}

            var user = _repo.GetUserByEmail(accountId);
            return user;
            //return new UserService(db, currentUser);
        }
    }
}
