namespace IranAudioGuide_MainServer.Migrations
{
    using Models;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<IranAudioGuide_MainServer.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(IranAudioGuide_MainServer.Models.ApplicationDbContext context)
        {
            var dbManager = new dbManager();
            // Create Roles
            dbManager.RoleCreator(context, "Admin");
            dbManager.RoleCreator(context, "AppUser");
            dbManager.RoleCreator(context, "Seller");

            //Add User
            dbManager.AddUser("webmoneyuser@iranaudioguide.com", "123456789", "/logo.png", "Webmoney User", "AppUser", context);
            dbManager.AddUser("enamaduser@iranaudioguide.com", "123456789", "/logo.png", "Enamad User", "AppUser", context);
            dbManager.AddUser("testuser@iranaudioguide.com", "1234567890", "/images/Members/Soroosh.JPG", "Soroosh Ansari", "AppUser", context);
            dbManager.AddUser("admin@iranaudioguide.com", "1234567890", "/images/Members/Mona.JPG", "Mona Akhlaghi", "Admin", context);
            dbManager.AddUser("alireza@iranaudioguide.com", "alirez@90", "/images/Members/AliReza.JPG", "Alireza Mottaghi", "Admin", context);
            dbManager.AddUser("appleuser@iranaudioguide.com", "123456789", "http://iranaudioguide.com/logo.png", "Apple User", "AppUser", context);
            
            dbManager.AddUser("seller@iranaudioguide.com", "123456789", "/logo.png", "Seller", "Seller", context);

            //AddTipCategory
            dbManager.AddTipCategory("Transportation", "ion-android-walk", "&#xf3bb;", 1);
            dbManager.AddTipCategory("Rough track", "ion-ios-pulse-strong", "&#xf492;", 2);
            dbManager.AddTipCategory("Time", "ion-android-time", "&#xf3b3;", 3);
            dbManager.AddTipCategory("Stuff", "ion-android-checkbox-outline", "&#xf373;", 4);
            dbManager.AddTipCategory("Other", "ion-ios-nutrition-outline", "&#xf46f;", 5);
        }
    }
}
