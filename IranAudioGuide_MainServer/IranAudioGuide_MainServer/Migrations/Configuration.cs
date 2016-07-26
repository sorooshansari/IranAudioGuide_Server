namespace IranAudioGuide_MainServer.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            // Create Roles
            RoleCreator(context, "Admin");
            RoleCreator(context, "AppUser");
            
            //Add User
            AddUser("sorosh.ansari@gmail.com", "1234567890", "/images/Members/Soroosh.JPG", "Soroosh Ansari", "Admin", context);
            AddUser("hakhlaghi@gmail.com", "1234567890", "/images/Members/Hamed.JPG", "Hamed Akhlaghi", "Admin", context);
            AddUser("monaakhlaghi@gmail.com", "1234567890", "/images/Members/Mona.JPG", "Mona Akhlaghi", "Admin", context);
            AddUser("sinazandi1994@gmail.com", "1234567890", "/images/Members/Sina.JPG", "Sina Zandi", "Admin", context);
        }
        private void AddUser(string Email, string Pass, string ImgUrl, string FullName, string Role, ApplicationDbContext context)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            if (UserManager.FindByName(Email) == null)
            {
                var user = new ApplicationUser()
                {
                    UserName = Email,
                    Email = Email,
                    FullName = FullName,
                    ImgUrl = ImgUrl
                };
                UserManager.Create(user, Pass);
                UserManager.AddToRole(user.Id, Role);
            }
        }
        private void RoleCreator(ApplicationDbContext context, string roleName)
        {
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            IdentityResult roleResult;

            // Check to see if Role Exists, if not create it
            if (!RoleManager.RoleExists(roleName))
            {
                roleResult = RoleManager.Create(new IdentityRole(roleName));
            }
        }
    }
}
