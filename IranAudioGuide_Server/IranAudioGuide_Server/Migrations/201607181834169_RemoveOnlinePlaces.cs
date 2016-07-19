namespace IranAudioGuide_Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveOnlinePlaces : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Audios", "OnlinePlace_OnP_Id", "dbo.OnlinePlaces");
            DropForeignKey("dbo.OnlinePlaces", "OnP_city_Cit_Id", "dbo.cities");
            DropForeignKey("dbo.Images", "OnlinePlace_OnP_Id", "dbo.OnlinePlaces");
            DropIndex("dbo.Audios", new[] { "OnlinePlace_OnP_Id" });
            DropIndex("dbo.Images", new[] { "OnlinePlace_OnP_Id" });
            DropIndex("dbo.OnlinePlaces", new[] { "OnP_city_Cit_Id" });
            AddColumn("dbo.Places", "Pla_isOnline", c => c.Boolean(nullable: false));
            DropColumn("dbo.Audios", "OnlinePlace_OnP_Id");
            DropColumn("dbo.Images", "OnlinePlace_OnP_Id");
            DropTable("dbo.OnlinePlaces");
        }
        
        public override void Down()
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
                        OnP_UpdateNumber = c.Int(nullable: false),
                        OnP_city_Cit_Id = c.Int(),
                    })
                .PrimaryKey(t => t.OnP_Id);
            
            AddColumn("dbo.Images", "OnlinePlace_OnP_Id", c => c.Guid());
            AddColumn("dbo.Audios", "OnlinePlace_OnP_Id", c => c.Guid());
            DropColumn("dbo.Places", "Pla_isOnline");
            CreateIndex("dbo.OnlinePlaces", "OnP_city_Cit_Id");
            CreateIndex("dbo.Images", "OnlinePlace_OnP_Id");
            CreateIndex("dbo.Audios", "OnlinePlace_OnP_Id");
            AddForeignKey("dbo.Images", "OnlinePlace_OnP_Id", "dbo.OnlinePlaces", "OnP_Id");
            AddForeignKey("dbo.OnlinePlaces", "OnP_city_Cit_Id", "dbo.cities", "Cit_Id");
            AddForeignKey("dbo.Audios", "OnlinePlace_OnP_Id", "dbo.OnlinePlaces", "OnP_Id");
        }
    }
}
