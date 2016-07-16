namespace IranAudioGuide_Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePlaceIndex7 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OnlinePlaces",
                c => new
                    {
                        OnP_Id = c.Guid(nullable: false, identity: true),
                        OnP_Name = c.String(),
                        OnP_ImgUrl = c.String(),
                        OnP_Discription = c.String(),
                        OnP_cordinate_X = c.Double(nullable: false),
                        OnP_cordinate_Y = c.Double(nullable: false),
                        OnP_Address = c.String(),
                        OnP_Deactive = c.Boolean(nullable: false),
                        OnP_city_Cit_Id = c.Int(),
                    })
                .PrimaryKey(t => t.OnP_Id)
                .ForeignKey("dbo.cities", t => t.OnP_city_Cit_Id)
                .Index(t => t.OnP_city_Cit_Id);
            
            AddColumn("dbo.Audios", "OnlinePlace_OnP_Id", c => c.Guid());
            AddColumn("dbo.Images", "OnlinePlace_OnP_Id", c => c.Guid());
            CreateIndex("dbo.Audios", "OnlinePlace_OnP_Id");
            CreateIndex("dbo.Images", "OnlinePlace_OnP_Id");
            AddForeignKey("dbo.Audios", "OnlinePlace_OnP_Id", "dbo.OnlinePlaces", "OnP_Id");
            AddForeignKey("dbo.Images", "OnlinePlace_OnP_Id", "dbo.OnlinePlaces", "OnP_Id");
            DropColumn("dbo.Places", "Pla_isOnline");
            DropColumn("dbo.Places", "Pla_Index");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Places", "Pla_Index", c => c.Int(nullable: false));
            AddColumn("dbo.Places", "Pla_isOnline", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.Images", "OnlinePlace_OnP_Id", "dbo.OnlinePlaces");
            DropForeignKey("dbo.OnlinePlaces", "OnP_city_Cit_Id", "dbo.cities");
            DropForeignKey("dbo.Audios", "OnlinePlace_OnP_Id", "dbo.OnlinePlaces");
            DropIndex("dbo.OnlinePlaces", new[] { "OnP_city_Cit_Id" });
            DropIndex("dbo.Images", new[] { "OnlinePlace_OnP_Id" });
            DropIndex("dbo.Audios", new[] { "OnlinePlace_OnP_Id" });
            DropColumn("dbo.Images", "OnlinePlace_OnP_Id");
            DropColumn("dbo.Audios", "OnlinePlace_OnP_Id");
            DropTable("dbo.OnlinePlaces");
        }
    }
}
