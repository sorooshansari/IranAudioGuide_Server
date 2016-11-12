namespace IranAudioGuide_MainServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateLog_updated2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UpdateLogs", "Ima_Id", c => c.Guid());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UpdateLogs", "Ima_Id");
        }
    }
}
