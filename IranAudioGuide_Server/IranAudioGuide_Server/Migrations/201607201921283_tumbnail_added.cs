namespace IranAudioGuide_Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tumbnail_added : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Places", "Pla_TumbImgUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Places", "Pla_TumbImgUrl");
        }
    }
}
