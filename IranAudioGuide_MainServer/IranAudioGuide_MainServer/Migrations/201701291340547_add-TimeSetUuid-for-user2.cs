namespace IranAudioGuide_MainServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTimeSetUuidforuser2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "TimeSetUuid", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "TimeSetUuid", c => c.DateTime(nullable: false));
        }
    }
}
