namespace IranAudioGuide_Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUpdateCounter : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OnlinePlaces", "OnP_UpdateNumber", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OnlinePlaces", "OnP_UpdateNumber");
        }
    }
}
