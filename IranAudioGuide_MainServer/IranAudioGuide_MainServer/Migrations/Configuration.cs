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
            dbManager.AddUser("testuser@iranaudioguide.com", "1234567890", "/images/Members/Soroosh.JPG", "Soroosh Ansari", "AppUser", context);
            dbManager.AddUser("admin@iranaudioguide.com", "1234567890", "/images/Members/Mona.JPG", "Mona Akhlaghi", "Admin", context);

            //AddTipCategory
            dbManager.AddTipCategory("Transportation", "ion-android-walk", "&#xf3bb;", 1);
            dbManager.AddTipCategory("Rough track", "ion-ios-pulse-strong", "&#xf492;", 2);
            dbManager.AddTipCategory("Time", "ion-android-time", "&#xf3b3;", 3);
            dbManager.AddTipCategory("Stuff", "ion-android-checkbox-outline", "&#xf373;", 4);
            dbManager.AddTipCategory("Other", "ion-ios-nutrition-outline", "&#xf46f;", 5);
        }

    }
}
