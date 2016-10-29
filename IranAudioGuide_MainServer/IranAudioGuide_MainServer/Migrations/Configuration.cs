namespace IranAudioGuide_MainServer.Migrations
{
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
            var dbManager = new dbManager();
            // Create Roles
            dbManager.RoleCreator(context, "Admin");
            dbManager.RoleCreator(context, "AppUser");

            //Add User
            dbManager.AddUser("sorosh.ansari@gmail.com", "1234567890", "/images/Members/Soroosh.JPG", "Soroosh Ansari", "Admin", context);
            dbManager.AddUser("hakhlaghi@gmail.com", "1234567890", "/images/Members/Hamed.JPG", "Hamed Akhlaghi", "Admin", context);
            dbManager.AddUser("monaakhlaghi@gmail.com", "1234567890", "/images/Members/Mona.JPG", "Mona Akhlaghi", "Admin", context);
            dbManager.AddUser("sinazandi1994@gmail.com", "1234567890", "/images/Members/Sina.JPG", "Sina Zandi", "Admin", context);

            //AddTipCategory
            AddTipCategory
        }

    }
}
