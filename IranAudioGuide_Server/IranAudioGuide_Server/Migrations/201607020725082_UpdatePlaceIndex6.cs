namespace IranAudioGuide_Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePlaceIndex6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Places", "Pla_Index", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Places", "Pla_Index");
        }
    }
}
