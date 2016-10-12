namespace IranAudioGuide_MainServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uuidAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "uuid", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "uuid");
        }
    }
}
