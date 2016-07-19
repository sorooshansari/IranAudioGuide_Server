namespace IranAudioGuide_Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UploadLogAdded : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Places", "Pla_UpdateNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Places", "Pla_UpdateNumber", c => c.Int(nullable: false));
        }
    }
}
