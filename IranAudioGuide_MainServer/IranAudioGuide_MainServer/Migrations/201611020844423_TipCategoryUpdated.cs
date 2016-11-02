namespace IranAudioGuide_MainServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TipCategoryUpdated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TipCategories", "TiC_Priority", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TipCategories", "TiC_Priority");
        }
    }
}
