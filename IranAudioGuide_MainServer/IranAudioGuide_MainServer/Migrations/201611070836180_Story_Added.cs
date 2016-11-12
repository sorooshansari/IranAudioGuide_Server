namespace IranAudioGuide_MainServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Story_Added : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Stories",
                c => new
                    {
                        Sto_Id = c.Guid(nullable: false, identity: true),
                        Sto_Name = c.String(),
                        Sto_Url = c.String(),
                        Sto_Discription = c.String(),
                        Pla_Id_Pla_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Sto_Id)
                .ForeignKey("dbo.Places", t => t.Pla_Id_Pla_Id)
                .Index(t => t.Pla_Id_Pla_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Stories", "Pla_Id_Pla_Id", "dbo.Places");
            DropIndex("dbo.Stories", new[] { "Pla_Id_Pla_Id" });
            DropTable("dbo.Stories");
        }
    }
}
