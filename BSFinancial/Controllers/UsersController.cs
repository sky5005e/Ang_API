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
    public class UsersController : ApiController
    {
        private BSFinancialRepository _repo;
        public UsersController(BSFinancialRepository repo)
        {
            _repo = repo;
        }

        [Authorize]
        public IHttpActionResult Get()
        {
            var u = AccountModel.GetCurrentUser(_repo);

            if (u != null)
            {
                var loans = _repo.GetUsersByCompany(u.CompanyId).ToList();
                return Ok(loans);
            }

            return null;

            //return _repo.GetUsersByCompany(Id);
        }
    }
}
