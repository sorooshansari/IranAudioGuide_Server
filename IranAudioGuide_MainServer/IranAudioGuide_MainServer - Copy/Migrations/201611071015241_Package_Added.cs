namespace IranAudioGuide_MainServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Package_Added : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Packages",
                c => new
                    {
                        Pac_Id = c.Guid(nullable: false, identity: true),
                        Pac_Name = c.String(),
                        Pac_Price = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Pac_Id);
            
            CreateTable(
                "dbo.Packagecities",
                c => new
                    {
                        Package_Pac_Id = c.Guid(nullable: false),
                        city_Cit_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Package_Pac_Id, t.city_Cit_Id })
                .ForeignKey("dbo.Packages", t => t.Package_Pac_Id, cascadeDelete: true)
                .ForeignKey("dbo.cities", t => t.city_Cit_Id, cascadeDelete: true)
                .Index(t => t.Package_Pac_Id)
                .Index(t => t.city_Cit_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Packagecities", "city_Cit_Id", "dbo.cities");
            DropForeignKey("dbo.Packagecities", "Package_Pac_Id", "dbo.Packages");
            DropIndex("dbo.Packagecities", new[] { "city_Cit_Id" });
            DropIndex("dbo.Packagecities", new[] { "Package_Pac_Id" });
            DropTable("dbo.Packagecities");
            DropTable("dbo.Packages");
        }
    }
}
