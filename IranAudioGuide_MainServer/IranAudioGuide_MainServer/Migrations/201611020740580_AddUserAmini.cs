namespace IranAudioGuide_MainServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserAmini : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tips",
                c => new
                    {
                        Tip_Id = c.Guid(nullable: false, identity: true),
                        Tip_Content = c.String(),
                        Tip_Category_TiC_Id = c.Guid(),
                        Place_Pla_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Tip_Id)
                .ForeignKey("dbo.TipCategories", t => t.Tip_Category_TiC_Id)
                .ForeignKey("dbo.Places", t => t.Place_Pla_Id)
                .Index(t => t.Tip_Category_TiC_Id)
                .Index(t => t.Place_Pla_Id);
            
            CreateTable(
                "dbo.TipCategories",
                c => new
                    {
                        TiC_Id = c.Guid(nullable: false, identity: true),
                        TiC_Class = c.String(),
                        TiC_Unicode = c.String(),
                        TiC_Name = c.String(),
                    })
                .PrimaryKey(t => t.TiC_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tips", "Place_Pla_Id", "dbo.Places");
            DropForeignKey("dbo.Tips", "Tip_Category_TiC_Id", "dbo.TipCategories");
            DropIndex("dbo.Tips", new[] { "Place_Pla_Id" });
            DropIndex("dbo.Tips", new[] { "Tip_Category_TiC_Id" });
            DropTable("dbo.TipCategories");
            DropTable("dbo.Tips");
        }
    }
}
