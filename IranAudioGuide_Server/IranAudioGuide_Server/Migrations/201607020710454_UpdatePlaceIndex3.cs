namespace IranAudioGuide_Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePlaceIndex3 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Places", "Pla_Index");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Places", "Pla_Index", c => c.Int(nullable: false));
        }
    }
}
