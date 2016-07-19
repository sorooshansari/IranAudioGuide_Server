namespace IranAudioGuide_Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UploadLogAdded2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UpdateLogs",
                c => new
                    {
                        UpL_Id = c.Int(nullable: false, identity: true),
                        Aud_Id = c.Guid(),
                        Pla_ID = c.Guid(),
                        Cit_ID = c.Guid(),
                    })
                .PrimaryKey(t => t.UpL_Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UpdateLogs");
        }
    }
}
