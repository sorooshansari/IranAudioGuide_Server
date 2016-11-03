namespace IranAudioGuide_MainServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TipVirtualPlaceId_added : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Tips", name: "Place_Pla_Id", newName: "Pla_Id_Pla_Id");
            RenameIndex(table: "dbo.Tips", name: "IX_Place_Pla_Id", newName: "IX_Pla_Id_Pla_Id");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Tips", name: "IX_Pla_Id_Pla_Id", newName: "IX_Place_Pla_Id");
            RenameColumn(table: "dbo.Tips", name: "Pla_Id_Pla_Id", newName: "Place_Pla_Id");
        }
    }
}
