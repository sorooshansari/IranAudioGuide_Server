namespace IranAudioGuide_Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class b : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Audios", new[] { "Aud_Id" });
            DropPrimaryKey("dbo.Audios");
            AlterColumn("dbo.Audios", "Aud_Id", c => c.Guid(nullable: false, identity: true));
            AddPrimaryKey("dbo.Audios", "Aud_Id");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Audios");
            AlterColumn("dbo.Audios", "Aud_Id", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.Audios", "Aud_Id");
            CreateIndex("dbo.Audios", "Aud_Id", unique: true);
        }
    }
}
