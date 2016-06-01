namespace IranAudioGuide_Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class c : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Audios", name: "Place_Pla_Id", newName: "Pla_Id_Pla_Id");
            RenameIndex(table: "dbo.Audios", name: "IX_Place_Pla_Id", newName: "IX_Pla_Id_Pla_Id");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Audios", name: "IX_Pla_Id_Pla_Id", newName: "IX_Place_Pla_Id");
            RenameColumn(table: "dbo.Audios", name: "Pla_Id_Pla_Id", newName: "Place_Pla_Id");
        }
    }
}
