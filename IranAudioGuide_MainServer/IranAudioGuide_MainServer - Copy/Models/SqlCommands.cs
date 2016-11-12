using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Migrations;

namespace IranAudioGuide_MainServer.Models
{
    public static class SqlCommands
    {
        public static string CreateDeleteProcedure(string name, string tableName, string tablePrefix, string tableIDType)
        {
            return string.Format(DeleteProcedure, name, tableName, tablePrefix, tableIDType);
        }
        static readonly string DeleteProcedure = @"
CREATE PROCEDURE [dbo].[{0}]
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
END";
        public static readonly List<string> FirstCommands = new List<string>()
        {
            @"
CREATE FUNCTION [dbo].[AllAudios]() 
RETURNS 
@Audios TABLE 
(
	Id UNIQUEIDENTIFIER,
	Name NVARCHAR(MAX),
	Url NVARCHAR(MAX),
	Descript NVARCHAR(MAX),
	PlaceId UNIQUEIDENTIFIER
)
AS
BEGIN
	INSERT @Audios
	SELECT [Aud_Id]
		    ,[Aud_Name]
		    ,[Aud_Url]
		    ,[Aud_Discription]
		    ,[Pla_Id_Pla_Id]
	    FROM [dbo].[Audios]
	RETURN 
END",
@"
CREATE FUNCTION [dbo].[AllCities]() 
RETURNS 
@Cities TABLE 
(
	Id INT,
	Name NVARCHAR(MAX)
)
AS
BEGIN
	INSERT @Cities
	SELECT [Cit_Id]
		  ,[Cit_Name]
	  FROM [dbo].[cities]
	RETURN 
END", @"
CREATE FUNCTION [dbo].[AllImages]() 
RETURNS 
@Place TABLE 
(
	Id UNIQUEIDENTIFIER,
	Url NVARCHAR(MAX),
	PlaceId UNIQUEIDENTIFIER,
	Descript NVARCHAR(MAX)
)
AS
BEGIN
	INSERT @Place
	SELECT [Img_Id]
		  ,[Img_Name]
		  ,[Pla_Id_Pla_Id]
		  ,[Img_Description]
	  FROM [dbo].[Images]
	RETURN 
END",@"

CREATE FUNCTION [dbo].[AllPlaces]() 
RETURNS 
@Place TABLE 
(
	Id UNIQUEIDENTIFIER,
	Name NVARCHAR(MAX),
	Url NVARCHAR(MAX),
	TNUrl NVARCHAR(MAX),
	Descript NVARCHAR(MAX),
	X FLOAT,
	Y FLOAT,
	Adr NVARCHAR(MAX),
	CityId INT
)
AS
BEGIN
	INSERT @Place
	SELECT [Pla_Id]
		  ,[Pla_Name]
		  ,[Pla_ImgUrl]
		  ,[Pla_TumbImgUrl]
		  ,[Pla_Discription]
		  ,[Pla_cordinate_X]
		  ,[Pla_cordinate_Y]
		  ,[Pla_Address]
		  ,[Pla_city_Cit_Id]
	  FROM [dbo].[Places]
	 WHERE Pla_Deactive = 0 AND
		   Pla_isOnline = 1
	RETURN 
END",@"

CREATE FUNCTION [dbo].[GetLastUpdate]() 
RETURNS INT
AS
BEGIN
	DECLARE @LastUpdateNumber AS INT
	SELECT @LastUpdateNumber = MAX([UpL_Id])
	  FROM [dbo].[UpdateLogs]
	RETURN @LastUpdateNumber
END",@"

CREATE PROCEDURE [dbo].[GetAll]
AS
BEGIN
	SELECT * FROM [dbo].AllPlaces()
	SELECT * FROM [dbo].AllAudios()
	SELECT * FROM [dbo].AllCities()
	SELECT * FROM [dbo].AllImages()
	SELECT [dbo].[GetLastUpdate]() AS LastUpdate
END",@"

CREATE PROCEDURE [dbo].[GetUpdates]
	@UpdateNumber AS INT
AS
BEGIN
	SELECT * FROM [dbo].AllPlaces() p
	WHERE p.Id IN
	(
		SELECT Pla_ID
		FROM [dbo].[UpdateLogs]
		WHERE UpL_Id > @UpdateNumber AND Pla_ID IS NOT NULL
	)
	SELECT * FROM [dbo].AllAudios() a
	WHERE a.Id IN
	(
		SELECT Aud_Id
		FROM [dbo].[UpdateLogs]
		WHERE UpL_Id > @UpdateNumber AND Aud_Id IS NOT NULL
	)
	SELECT * FROM [dbo].AllCities() c
	WHERE c.Id IN
	(
		SELECT Cit_ID
		FROM [dbo].[UpdateLogs]
		WHERE UpL_Id > @UpdateNumber AND Cit_ID IS NOT NULL
	)
	SELECT * FROM [dbo].AllImages() i
	WHERE i.Id IN
	(
		SELECT Img_ID
		FROM [dbo].[UpdateLogs]
		WHERE UpL_Id > @UpdateNumber AND Img_ID IS NOT NULL
	)
	SELECT [dbo].[GetLastUpdate]() AS LastUpdate
END"
        };
    }
}