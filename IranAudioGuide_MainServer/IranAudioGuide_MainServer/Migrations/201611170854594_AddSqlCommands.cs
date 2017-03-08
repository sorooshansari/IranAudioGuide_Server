namespace IranAudioGuide_MainServer.Migrations
{
    using Models;
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddSqlCommands : DbMigration
    {
        public override void Up()
        {
            Sql(SqlCommands.CreateDeleteProcedure("DeleteCity", "cities", "Cit", "int"));
            Sql(SqlCommands.CreateDeleteProcedure("DeletePlace", "Places", "Pla", "uniqueidentifier"));
            foreach (var command in SqlCommands.FirstCommands)
                Sql(command);
            foreach (var command in SqlCommands.ElamSqlCommands)
                Sql(command);
        }

        public override void Down()
        {
        }
    }
}
