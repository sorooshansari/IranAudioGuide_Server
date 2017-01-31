namespace IranAudioGuide_MainServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTimeSetUuidforuser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "TimeSetUuid", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "TimeSetUuid");
        }
    }
}
