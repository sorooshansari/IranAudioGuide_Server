namespace IranAudioGuide_MainServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Place_isPrimary_added : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Places", "Pla_isPrimary", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Places", "Pla_isPrimary");
        }
    }
}
