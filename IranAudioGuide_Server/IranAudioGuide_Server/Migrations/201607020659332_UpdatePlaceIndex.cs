namespace IranAudioGuide_Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePlaceIndex : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Places", "Pla_Index", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Places", "Pla_Index", c => c.Int(nullable: false, identity: true));
        }
    }
}
