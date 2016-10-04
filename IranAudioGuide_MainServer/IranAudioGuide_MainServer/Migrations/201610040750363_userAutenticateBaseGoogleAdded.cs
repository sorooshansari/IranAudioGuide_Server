namespace IranAudioGuide_MainServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userAutenticateBaseGoogleAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "GoogleId", c => c.String());
            AddColumn("dbo.AspNetUsers", "gender", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "Picture", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Picture");
            DropColumn("dbo.AspNetUsers", "gender");
            DropColumn("dbo.AspNetUsers", "GoogleId");
        }
    }
}
