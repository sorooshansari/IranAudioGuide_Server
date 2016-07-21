namespace IranAudioGuide_Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LOGUPDATED : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UpdateLogs", "Img_Id", c => c.Guid());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UpdateLogs", "Img_Id");
        }
    }
}
