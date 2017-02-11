namespace IranAudioGuide_MainServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTabelRequestForAppComments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Comment_Id = c.Int(nullable: false, identity: true),
                        Subject = c.String(),
                        Message = c.String(),
                        CreateTime = c.DateTime(nullable: false),
                        IsRead = c.Boolean(nullable: false),
                        uuid = c.String(),
                    })
                .PrimaryKey(t => t.Comment_Id);
            
            CreateTable(
                "dbo.RequestForApps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(),
                        CreateRequest = c.DateTime(nullable: false),
                        IsSend = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RequestForApps");
            DropTable("dbo.Comments");
        }
    }
}
