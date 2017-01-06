namespace IranAudioGuide_MainServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tempUserTableAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TempUsers",
                c => new
                    {
                        TeU_Id = c.Guid(nullable: false, identity: true),
                        TeU_UUId = c.String(),
                    })
                .PrimaryKey(t => t.TeU_Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TempUsers");
        }
    }
}
