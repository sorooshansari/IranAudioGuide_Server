namespace IranAudioGuide_Server.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<IranAudioGuide_Server.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "IranAudioGuide_Server.Models.ApplicationDbContext";
        }

        protected override void Seed(IranAudioGuide_Server.Models.ApplicationDbContext context)
        {
            // Create Roles
            RoleCreator(context, "Admin");

            //Add User
            AddUser("sorosh.ansari@gmail.com", "1234567890", "/images/Members/Soroosh.JPG", "Soroosh Ansari", "Admin", context);
            AddUser("hakhlaghi@gmail.com", "1234567890", "/images/Members/Hamed.JPG", "Hamed Akhlaghi", "Admin", context);
            AddUser("monaakhlaghi@gmail.com", "1234567890", "/images/Members/Mona.JPG", "Mona Akhlaghi", "Admin", context);
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
        private void AddUser(string Email, string Pass, string ImgUrl, string FullName, string Role, IranAudioGuide_Server.Models.ApplicationDbContext context)
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
        private void RoleCreator(IranAudioGuide_Server.Models.ApplicationDbContext context, string roleName)
        {
            //var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
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
