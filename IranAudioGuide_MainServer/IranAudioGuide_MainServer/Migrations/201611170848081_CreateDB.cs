namespace IranAudioGuide_MainServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Audios",
                c => new
                    {
                        Aud_Id = c.Guid(nullable: false, identity: true),
                        Aud_Name = c.String(),
                        Aud_Url = c.String(),
                        Aud_Discription = c.String(),
                        Pla_Id_Pla_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Aud_Id)
                .ForeignKey("dbo.Places", t => t.Pla_Id_Pla_Id)
                .Index(t => t.Pla_Id_Pla_Id);
            
            CreateTable(
                "dbo.Places",
                c => new
                    {
                        Pla_Id = c.Guid(nullable: false, identity: true),
                        Pla_Name = c.String(),
                        Pla_ImgUrl = c.String(),
                        Pla_TumbImgUrl = c.String(),
                        Pla_Discription = c.String(),
                        Pla_cordinate_X = c.Double(nullable: false),
                        Pla_cordinate_Y = c.Double(nullable: false),
                        Pla_Address = c.String(),
                        Pla_Deactive = c.Boolean(nullable: false),
                        Pla_isOnline = c.Boolean(nullable: false),
                        Pla_isPrimary = c.Boolean(nullable: false),
                        Pla_city_Cit_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Pla_Id)
                .ForeignKey("dbo.cities", t => t.Pla_city_Cit_Id)
                .Index(t => t.Pla_city_Cit_Id);
            
            CreateTable(
                "dbo.cities",
                c => new
                    {
                        Cit_Id = c.Int(nullable: false, identity: true),
                        Cit_Name = c.String(),
                        Cit_Description = c.String(),
                    })
                .PrimaryKey(t => t.Cit_Id);
            
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
                "dbo.Images",
                c => new
                    {
                        Img_Id = c.Guid(nullable: false, identity: true),
                        Img_Name = c.String(),
                        Img_Description = c.String(),
                        Pla_Id_Pla_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Img_Id)
                .ForeignKey("dbo.Places", t => t.Pla_Id_Pla_Id)
                .Index(t => t.Pla_Id_Pla_Id);
            
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
            
            CreateTable(
                "dbo.Tips",
                c => new
                    {
                        Tip_Id = c.Guid(nullable: false, identity: true),
                        Tip_Content = c.String(),
                        Pla_Id_Pla_Id = c.Guid(),
                        Tip_Category_TiC_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Tip_Id)
                .ForeignKey("dbo.Places", t => t.Pla_Id_Pla_Id)
                .ForeignKey("dbo.TipCategories", t => t.Tip_Category_TiC_Id)
                .Index(t => t.Pla_Id_Pla_Id)
                .Index(t => t.Tip_Category_TiC_Id);
            
            CreateTable(
                "dbo.TipCategories",
                c => new
                    {
                        TiC_Id = c.Guid(nullable: false, identity: true),
                        TiC_Class = c.String(),
                        TiC_Unicode = c.String(),
                        TiC_Name = c.String(),
                        TiC_Priority = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TiC_Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.UpdateLogs",
                c => new
                    {
                        UpL_Id = c.Int(nullable: false, identity: true),
                        Aud_Id = c.Guid(),
                        Pla_ID = c.Guid(),
                        Cit_ID = c.Int(),
                        Img_Id = c.Guid(),
                        Sto_Id = c.Guid(),
                        Tip_Id = c.Guid(),
                        Ima_Id = c.Guid(),
                        isRemoved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UpL_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FullName = c.String(),
                        ImgUrl = c.String(),
                        GoogleId = c.String(),
                        gender = c.Int(nullable: false),
                        Picture = c.String(),
                        uuid = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
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
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Tips", "Tip_Category_TiC_Id", "dbo.TipCategories");
            DropForeignKey("dbo.Tips", "Pla_Id_Pla_Id", "dbo.Places");
            DropForeignKey("dbo.Stories", "Pla_Id_Pla_Id", "dbo.Places");
            DropForeignKey("dbo.Images", "Pla_Id_Pla_Id", "dbo.Places");
            DropForeignKey("dbo.Places", "Pla_city_Cit_Id", "dbo.cities");
            DropForeignKey("dbo.Packagecities", "city_Cit_Id", "dbo.cities");
            DropForeignKey("dbo.Packagecities", "Package_Pac_Id", "dbo.Packages");
            DropForeignKey("dbo.Audios", "Pla_Id_Pla_Id", "dbo.Places");
            DropIndex("dbo.Packagecities", new[] { "city_Cit_Id" });
            DropIndex("dbo.Packagecities", new[] { "Package_Pac_Id" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Tips", new[] { "Tip_Category_TiC_Id" });
            DropIndex("dbo.Tips", new[] { "Pla_Id_Pla_Id" });
            DropIndex("dbo.Stories", new[] { "Pla_Id_Pla_Id" });
            DropIndex("dbo.Images", new[] { "Pla_Id_Pla_Id" });
            DropIndex("dbo.Places", new[] { "Pla_city_Cit_Id" });
            DropIndex("dbo.Audios", new[] { "Pla_Id_Pla_Id" });
            DropTable("dbo.Packagecities");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.UpdateLogs");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.TipCategories");
            DropTable("dbo.Tips");
            DropTable("dbo.Stories");
            DropTable("dbo.Images");
            DropTable("dbo.Packages");
            DropTable("dbo.cities");
            DropTable("dbo.Places");
            DropTable("dbo.Audios");
        }
    }
}
