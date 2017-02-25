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
		    ,[Place_Pla_Id]
	    FROM [dbo].[Audios]
	RETURN 
END",
@"

CREATE FUNCTION [dbo].[AllCities]() 
RETURNS 
@Cities TABLE 
(
	Id INT,
	Name NVARCHAR(MAX),
	Descript NVARCHAR(MAX),
	ImageUrl NVARCHAR(MAX)
)
AS
BEGIN
	INSERT @Cities
	SELECT [Cit_Id]
		  ,[Cit_Name]
		  ,[Cit_Description]
		  ,[Cit_ImageUrl]
	  FROM [dbo].[cities]
	RETURN 
END",
@"
CREATE PROCEDURE [dbo].[CreateComment] 
	@Message NVARCHAR(MAX),
	@uuid NVARCHAR(MAX),
	@Subject NVARCHAR(MAX),
	@email NVARCHAR(MAX)
AS
BEGIN
	INSERT INTO [dbo].[Comments] (Subject, Message, CreateTime, Email, IsRead, uuid)
VALUES        (@Subject ,@Message, getdate(), @email, 'False' ,@uuid)
	
END
", @"

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
		  ,[Place_Pla_Id]
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
	CityId INT,
    isPrimary BIT
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
		  ,[Pla_isPrimary]
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
END", @"

CREATE FUNCTION [dbo].[AllStories]()
RETURNS
@Stories TABLE 
(
	Id UNIQUEIDENTIFIER,
	Name NVARCHAR(MAX),
	Url NVARCHAR(MAX),
	Descript NVARCHAR(MAX),
	PlaceId UNIQUEIDENTIFIER
)
AS
BEGIN
	INSERT @Stories
	SELECT [Sto_Id]
		    ,[Sto_Name]
		    ,[Sto_Url]
		    ,[Sto_Discription]
		    ,[Place_Pla_Id]
	    FROM [dbo].[Stories]
	RETURN 
END", @"

CREATE FUNCTION [dbo].[AllTipCategories]() 
RETURNS 
@TipCategories TABLE 
(
	Id UNIQUEIDENTIFIER,
	Class NVARCHAR(MAX),
	TipUnicode NVARCHAR(MAX),
	Name NVARCHAR(MAX),
	TipPriority INT
)
AS
BEGIN
	INSERT @TipCategories
	SELECT [TiC_Id]
		  ,[TiC_Class]
		  ,[TiC_Unicode]
		  ,[TiC_Name]
		  ,[TiC_Priority]
	  FROM [dbo].[TipCategories]
	RETURN 
END", @"

CREATE FUNCTION [dbo].[AllTips]() 
RETURNS 
@Tips TABLE 
(
	Id UNIQUEIDENTIFIER,
	Content NVARCHAR(MAX),
	CategoryId UNIQUEIDENTIFIER,
	PlaceId UNIQUEIDENTIFIER
)
AS
BEGIN
	INSERT @Tips
	SELECT [Tip_Id]
		  ,[Tip_Content]
		  ,[Tip_Category_TiC_Id]
		  ,[Place_Pla_Id]
	  FROM [dbo].[Tips]
	RETURN 
END",@"

CREATE PROCEDURE [dbo].[GetAll]
AS
BEGIN
	SELECT [dbo].[GetLastUpdate]() AS LastUpdate

	SELECT * FROM [dbo].AllPlaces() AS Places

	SELECT * FROM [dbo].AllAudios() AS Audios
	WHERE Audios.PlaceId IN
		(SELECT Id FROM [dbo].AllPlaces())
	
	SELECT * FROM [dbo].AllStories() AS Stories
	WHERE Stories.PlaceId IN
		(SELECT Id FROM [dbo].AllPlaces())
	
	SELECT * FROM [dbo].AllImages() AS Images
	WHERE Images.PlaceId IN
		(SELECT Id FROM [dbo].AllPlaces())
		
	SELECT * FROM [dbo].AllTips() AS Tips
	WHERE Tips.PlaceId IN
		(SELECT Id FROM [dbo].AllPlaces())

	SELECT * FROM [dbo].AllCities()

	SELECT * FROM [dbo].AllTipCategories()
END",@"

CREATE PROCEDURE [dbo].[GetUpdates]
	@UpdateNumber AS INT
AS
BEGIN
	SELECT [dbo].[GetLastUpdate]() AS LastUpdate
	--GETING NEW ENTITIES
	SELECT * FROM [dbo].AllPlaces() p
	WHERE p.Id IN
	(
		SELECT Pla_ID
		FROM [dbo].[UpdateLogs]
		WHERE UpL_Id > @UpdateNumber AND Pla_ID IS NOT NULL AND isRemoved = 0
	)

	SELECT * FROM [dbo].AllAudios() a
	WHERE a.Id IN
	(
		SELECT Aud_Id
		FROM [dbo].[UpdateLogs]
		WHERE UpL_Id > @UpdateNumber AND Aud_Id IS NOT NULL AND isRemoved = 0
	) OR a.PlaceId IN
	(
		SELECT Pla_ID
		FROM [dbo].[UpdateLogs]
		WHERE UpL_Id > @UpdateNumber AND Pla_ID IS NOT NULL AND isRemoved = 0
	)
	
	SELECT * FROM [dbo].AllStories() s
	WHERE s.Id IN
	(
		SELECT Sto_Id
		FROM [dbo].[UpdateLogs]
		WHERE UpL_Id > @UpdateNumber AND Sto_Id IS NOT NULL AND isRemoved = 0
	) OR s.PlaceId IN
	(
		SELECT Pla_ID
		FROM [dbo].[UpdateLogs]
		WHERE UpL_Id > @UpdateNumber AND Pla_ID IS NOT NULL AND isRemoved = 0
	)

	SELECT * FROM [dbo].AllImages() i
	WHERE i.Id IN
	(
		SELECT Img_ID
		FROM [dbo].[UpdateLogs]
		WHERE UpL_Id > @UpdateNumber AND Img_ID IS NOT NULL AND isRemoved = 0
	) OR i.PlaceId IN
	(
		SELECT Pla_ID
		FROM [dbo].[UpdateLogs]
		WHERE UpL_Id > @UpdateNumber AND Pla_ID IS NOT NULL AND isRemoved = 0
	)

	SELECT * FROM [dbo].AllTips() t
	WHERE t.Id IN
	(
		SELECT Tip_ID
		FROM [dbo].[UpdateLogs]
		WHERE UpL_Id > @UpdateNumber AND Tip_ID IS NOT NULL AND isRemoved = 0
	) OR t.PlaceId IN
	(
		SELECT Pla_ID
		FROM [dbo].[UpdateLogs]
		WHERE UpL_Id > @UpdateNumber AND Pla_ID IS NOT NULL AND isRemoved = 0
	)

	SELECT * FROM [dbo].AllCities() c
	WHERE c.Id IN
	(
		SELECT Cit_ID
		FROM [dbo].[UpdateLogs]
		WHERE UpL_Id > @UpdateNumber AND Cit_ID IS NOT NULL AND isRemoved = 0
	)
	--GETING REMOVED ENTITIES
	SELECT Pla_ID AS Id
	FROM [dbo].[UpdateLogs]
	WHERE UpL_Id > @UpdateNumber AND Pla_ID IS NOT NULL AND isRemoved = 1

	SELECT a.Id AS Id FROM [dbo].AllAudios() a
	WHERE a.Id IN
	(
		SELECT Aud_Id
		FROM [dbo].[UpdateLogs]
		WHERE UpL_Id > @UpdateNumber AND Aud_Id IS NOT NULL AND isRemoved = 1
	) OR a.PlaceId IN
	(
		SELECT Pla_ID
		FROM [dbo].[UpdateLogs]
		WHERE UpL_Id > @UpdateNumber AND Pla_ID IS NOT NULL AND isRemoved = 1
	)
	
	SELECT s.Id AS Id FROM [dbo].AllStories() s
	WHERE s.Id IN
	(
		SELECT Sto_Id
		FROM [dbo].[UpdateLogs]
		WHERE UpL_Id > @UpdateNumber AND Sto_Id IS NOT NULL AND isRemoved = 1
	) OR s.PlaceId IN
	(
		SELECT Pla_ID
		FROM [dbo].[UpdateLogs]
		WHERE UpL_Id > @UpdateNumber AND Pla_ID IS NOT NULL AND isRemoved = 1
	)

	SELECT i.Id AS Id FROM [dbo].AllImages() i
	WHERE i.Id IN
	(
		SELECT Img_ID
		FROM [dbo].[UpdateLogs]
		WHERE UpL_Id > @UpdateNumber AND Img_ID IS NOT NULL AND isRemoved = 1
	) OR i.PlaceId IN
	(
		SELECT Pla_ID
		FROM [dbo].[UpdateLogs]
		WHERE UpL_Id > @UpdateNumber AND Pla_ID IS NOT NULL AND isRemoved = 1
	)

	SELECT t.Id AS Id FROM [dbo].AllTips() t
	WHERE t.Id IN
	(
		SELECT Tip_ID
		FROM [dbo].[UpdateLogs]
		WHERE UpL_Id > @UpdateNumber AND Tip_ID IS NOT NULL AND isRemoved = 1
	) OR t.PlaceId IN
	(
		SELECT Pla_ID
		FROM [dbo].[UpdateLogs]
		WHERE UpL_Id > @UpdateNumber AND Pla_ID IS NOT NULL AND isRemoved = 1
	)

	SELECT c.Id AS Id FROM [dbo].AllCities() c
	WHERE c.Id IN
	(
		SELECT Cit_ID
		FROM [dbo].[UpdateLogs]
		WHERE UpL_Id > @UpdateNumber AND Cit_ID IS NOT NULL AND isRemoved = 1
	)
END",@"
CREATE PROCEDURE GetAutorizedCities
	@UserID AS nvarchar(128)
AS
BEGIN
	SET NOCOUNT ON;

    SELECT dbo.Packagecities.city_Cit_Id AS cityID
	FROM  dbo.Packagecities INNER JOIN
			 dbo.Packages ON dbo.Packagecities.Package_Pac_Id = dbo.Packages.Pac_Id INNER JOIN
			 dbo.Payments ON dbo.Packages.Pac_Id = dbo.Payments.Package_Pac_Id INNER JOIN
			 dbo.AspNetUsers ON dbo.Payments.User_Id = dbo.AspNetUsers.Id
	WHERE (dbo.AspNetUsers.Id = @UserID) AND (dbo.Payments.PaymentFinished = 1)
	GROUP BY dbo.Packagecities.city_Cit_Id
END",
@"CREATE PROCEDURE [dbo].[GetPackages]
	@CityId int
AS
BEGIN
	DECLARE @Packages TABLE 
	(
		Id uniqueidentifier,
		Name nvarchar(MAX),
		Price bigint
	)
	DECLARE @Cities TABLE 
	(
		Id INT
	)
	DECLARE @ResultCities TABLE 
	(
		Id INT,
		AudioCount INT,
		StoryCount INT
	)
	DECLARE @PackageCities TABLE 
	(
		Id INT,
		CityName NVARCHAR(MAX),
		PlacesCount INT,
		AudiosCount INT,
		StoriesCount INT
	)

	INSERT @Packages
	SELECT dbo.Packages.Pac_Id, dbo.Packages.Pac_Name, dbo.Packages.Pac_Price
	  FROM dbo.Packagecities INNER JOIN
				dbo.Packages ON dbo.Packagecities.Package_Pac_Id = dbo.Packages.Pac_Id
	 WHERE dbo.Packagecities.city_Cit_Id = @CityId
	 
	INSERT @Cities
	SELECT DISTINCT city_Cit_Id
	FROM  dbo.Packagecities 
	WHERE Package_Pac_Id IN 
		(SELECT Id FROM @Packages)

	DECLARE @TempCityId INT

	DECLARE City_CURSOR CURSOR 
	  LOCAL STATIC READ_ONLY FORWARD_ONLY
	FOR 
	SELECT DISTINCT Id
	FROM @Cities
	
	OPEN City_CURSOR
	FETCH NEXT FROM City_CURSOR INTO @TempCityId
	WHILE @@FETCH_STATUS = 0
	BEGIN 
		INSERT @ResultCities
		SELECT @TempCityId, dbo.AudiosCount(P.Id), dbo.StoriesCount(P.Id)
		FROM dbo.AllPlacesId(@TempCityId) AS P
		GROUP BY P.Id

		FETCH NEXT FROM City_CURSOR INTO @TempCityId
	END
	CLOSE City_CURSOR
	DEALLOCATE City_CURSOR
	
	INSERT @PackageCities
	SELECT Id, dbo.cities.Cit_Name, COUNT(*), SUM(AudioCount), SUM(StoryCount)
	FROM @ResultCities INNER JOIN
	dbo.cities ON dbo.cities.Cit_Id = Id
	GROUP BY Id, dbo.cities.Cit_Name

	SELECT * 
	FROM @Packages

	SELECT Package_Pac_Id AS PackageId, Id As CityId, CityName, PlacesCount, AudiosCount, StoriesCount
	FROM dbo.Packagecities right join
	@PackageCities ON city_Cit_Id = Id
	WHERE Package_Pac_Id IN (SELECT Id FROM @Packages)
END"
        };
    }
}