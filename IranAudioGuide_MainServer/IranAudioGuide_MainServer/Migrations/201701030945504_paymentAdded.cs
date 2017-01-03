namespace IranAudioGuide_MainServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class paymentAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        PaymentId = c.Guid(nullable: false, identity: true),
                        ReferenceNumber = c.String(maxLength: 100),
                        SaleReferenceId = c.Long(nullable: false),
                        StatusPayment = c.String(maxLength: 100),
                        PaymentFinished = c.Boolean(nullable: false),
                        Amount = c.Long(nullable: false),
                        BankName = c.String(maxLength: 50),
                        InsertDatetime = c.DateTime(nullable: false),
                        Package_Pac_Id = c.Guid(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.PaymentId)
                .ForeignKey("dbo.Packages", t => t.Package_Pac_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.Package_Pac_Id)
                .Index(t => t.User_Id);
            
            AlterColumn("dbo.Packages", "Pac_Price", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Payments", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Payments", "Package_Pac_Id", "dbo.Packages");
            DropIndex("dbo.Payments", new[] { "User_Id" });
            DropIndex("dbo.Payments", new[] { "Package_Pac_Id" });
            AlterColumn("dbo.Packages", "Pac_Price", c => c.Int(nullable: false));
            DropTable("dbo.Payments");
        }
    }
}
