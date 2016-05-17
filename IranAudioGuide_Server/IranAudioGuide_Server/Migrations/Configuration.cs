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
        private string _AdminUser;
        string AdminUser
        {
            get
            {
                return _AdminUser ?? "Admin";
            }
            set
            {
                _AdminUser = value;
            }
        }
        private string _AdminPass;
        string AdminPass
        {
            get
            {
                return _AdminPass ?? "123456789";
            }
            set
            {
                _AdminPass = value;
            }
        }
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
            AddUser(AdminUser, AdminPass, "Admin", context);
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
        private void AddUser(string Name, string Pass, string Role, IranAudioGuide_Server.Models.ApplicationDbContext context)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            if (UserManager.FindByName(Name) == null)
            {
                var user = new ApplicationUser() { UserName = Name };
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
