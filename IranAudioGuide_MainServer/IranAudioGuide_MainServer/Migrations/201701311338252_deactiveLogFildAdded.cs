namespace IranAudioGuide_MainServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deactiveLogFildAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LogUserFailures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IpAddress = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.AspNetUsers", "TimeSetUuid", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "TimeSetUuid");
            DropTable("dbo.LogUserFailures");
        }
    }
}
