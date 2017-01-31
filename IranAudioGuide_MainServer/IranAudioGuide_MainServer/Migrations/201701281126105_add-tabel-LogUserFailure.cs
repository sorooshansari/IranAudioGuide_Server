namespace IranAudioGuide_MainServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtabelLogUserFailure : DbMigration
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
            
        }
        
        public override void Down()
        {
            DropTable("dbo.LogUserFailures");
        }
    }
}
