namespace IranAudioGuide_Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePlace : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Places", "Pla_isOnline", c => c.Boolean(nullable: false));
            AddColumn("dbo.Places", "Pla_Deactive", c => c.Boolean(nullable: false));
            AddColumn("dbo.Places", "Pla_Index", c => c.Int(nullable: false, identity: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Places", "Pla_Index");
            DropColumn("dbo.Places", "Pla_Deactive");
            DropColumn("dbo.Places", "Pla_isOnline");
        }
    }
}
