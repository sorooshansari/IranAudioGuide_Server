namespace IranAudioGuide_Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UploadLogAdded3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UpdateLogs", "Cit_ID", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UpdateLogs", "Cit_ID", c => c.Guid());
        }
    }
}
