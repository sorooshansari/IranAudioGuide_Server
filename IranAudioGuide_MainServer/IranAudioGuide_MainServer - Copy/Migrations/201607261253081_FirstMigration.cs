namespace IranAudioGuide_MainServer.Migrations
{
    using Models;
    using System;
    using System.Data.Entity.Migrations;

    public partial class FirstMigration : DbMigration
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
                    })
                .PrimaryKey(t => t.UpL_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FullName = c.String(),
                        ImgUrl = c.String(),
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
            
            //Sql(SqlCommands.CreateDeleteProcedure("DeleteCity", "cities", "Cit", "int"));
            //Sql(SqlCommands.CreateDeleteProcedure("DeletePlace", "Places", "Pla", "uniqueidentifier"));
            //foreach (var command in SqlCommands.FirstCommands)
            //    Sql(command);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Images", "Pla_Id_Pla_Id", "dbo.Places");
            DropForeignKey("dbo.Places", "Pla_city_Cit_Id", "dbo.cities");
            DropForeignKey("dbo.Audios", "Pla_Id_Pla_Id", "dbo.Places");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Images", new[] { "Pla_Id_Pla_Id" });
            DropIndex("dbo.Places", new[] { "Pla_city_Cit_Id" });
            DropIndex("dbo.Audios", new[] { "Pla_Id_Pla_Id" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.UpdateLogs");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Images");
            DropTable("dbo.cities");
            DropTable("dbo.Places");
            DropTable("dbo.Audios");
        }
    }
}
