namespace IranAudioGuide_Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_des_to_image : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Images", "Img_Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Images", "Img_Description");
        }
    }
}
