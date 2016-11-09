namespace IranAudioGuide_MainServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateLog_updated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UpdateLogs", "Sto_Id", c => c.Guid());
            AddColumn("dbo.UpdateLogs", "Tip_Id", c => c.Guid());
            AddColumn("dbo.UpdateLogs", "isRemoved", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UpdateLogs", "isRemoved");
            DropColumn("dbo.UpdateLogs", "Tip_Id");
            DropColumn("dbo.UpdateLogs", "Sto_Id");
        }
    }
}
