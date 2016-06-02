namespace IranAudioGuide_Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sdAlter : DbMigration
    {
        public override void Up()
        {
            string DeleteProcedure = @"
ALTER PROCEDURE {0} 
    @Id {3}
AS
BEGIN
    DECLARE @ErrorNumber INT
    BEGIN TRY
	    DELETE FROM dbo.{1}
	    WHERE {2}_Id = @Id

	    SELECT 0
    END TRY
    BEGIN CATCH
	    SELECT @ErrorNumber = ERROR_NUMBER()
	    IF @ErrorNumber = 547
		    SELECT 1
	    ELSE
		    SELECT 2
    END CATCH
END
";
            //{0} ==> procedure name
            //{1} ==> table name
            //{2} ==> table prefix
            //{3} ==> table Id type
            Sql(string.Format(DeleteProcedure, "DeleteCity", "cities", "Cit", "int"));
            Sql(string.Format(DeleteProcedure, "DeletePlace", "Places", "Pla", "uniqueidentifier"));
        }
        
        public override void Down()
        {
        }
    }
}
