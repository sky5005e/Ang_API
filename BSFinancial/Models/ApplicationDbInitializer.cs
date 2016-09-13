using System.Data.Entity;
using System.Diagnostics;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace BSFinancial.Models
{
	public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
	{

		protected override void Seed(ApplicationDbContext context)
		{
			
			base.Seed(context);
		}

         //Create User=Admin@Admin.com with password=Admin@123456 in the Admin role        
        public static void InitializeIdentityForEF(ApplicationDbContext db)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
           // var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
        }

        //public static void InitializeIdentityForEF(ApplicationDbContext db)
        //{
        //    Debug.WriteLine("Initializing Identity");
        //    var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
        //    //var roleManager = new ApplicationRoleManager(new RoleStore<IdentityRole>(db));
        //    //var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //    //var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();


        //    const string name = "dheeraj@singal.net";

        //    const string password = "Asing123;";

        //    //const string roleName = "Admin";


        //    //Create Role Admin if it does not exist

        //    //var role = roleManager.FindByName(roleName);

        //    //if (role == null) {
        //    //    role = new IdentityRole(roleName);

        //    //    var roleresult = roleManager.Create(role);
        //    //}


        //    var user = userManager.FindByName(name);

        //    if (user == null) {
        //        user = new ApplicationUser {UserName = name, Email = name};

        //        var result = userManager.Create(user, password);

        //        result = userManager.SetLockoutEnabled(user.Id, false);
        //    }


        //    // Add user admin to Role Admin if not already added

        //    //var rolesForUser = userManager.GetRoles(user.Id);

        //    //if (!rolesForUser.Contains(role.Name)) {
        //    //    var result = userManager.AddToRole(user.Id, role.Name);
        //    //}



        //}
	}
}