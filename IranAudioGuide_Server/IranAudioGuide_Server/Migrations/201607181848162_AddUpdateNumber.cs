namespace IranAudioGuide_Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Places", "Pla_UpdateNumber", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Places", "Pla_UpdateNumber");
        }
    }
}
