using BSFinancial.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;

namespace BSFinancial.Models
{
    public class AccountModel
    {
        public static User GetCurrentUser(BSFinancialRepository repo)
        {
            string email = HttpContext.Current.User.Identity.GetUserName();
            if (email != null && email != "")
            {
                var user = repo.GetUserByEmail(email);
                return user;

            }
            else return null;

        }

        public static User GetCurrentUser(IBSFinancialRepository repo)
        {
            string email = HttpContext.Current.User.Identity.GetUserName();
            if (email != null && email != "")
            {
                var user = repo.GetUserByEmail(email);
                return user;
            }
            else return null;

        }
    }
}