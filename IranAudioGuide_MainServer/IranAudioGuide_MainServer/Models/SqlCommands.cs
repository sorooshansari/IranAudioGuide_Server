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
	@uuid NVARCHAR(MAX)
AS
BEGIN
	INSERT INTO [dbo].[UserLogs] (UsL_UUId, UsL_DateTime)
	VALUES (@uuid, getdate())

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
	@UpdateNumber AS INT,
	@uuid NVARCHAR(MAX)
AS
BEGIN
	INSERT INTO [dbo].[UserLogs] (UsL_UUId, UsL_DateTime)
	VALUES (@uuid, getdate())

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
CREATE PROCEDURE [dbo].[GetAutorizedCities]
	@UserID AS nvarchar(128)
AS
BEGIN
	SET NOCOUNT ON;

    SELECT Packagecities.city_Cit_Id AS cityID
      FROM Procurements INNER JOIN
	       Packagecities ON Procurements.Pac_Id = Packagecities.Package_Pac_Id
     WHERE 
	       (Procurements.Id = @UserID) AND (Pro_PaymentFinished = 1)
     GROUP BY Packagecities.city_Cit_Id

END",
@"CREATE FUNCTION [dbo].[AudiosCount]
(
	@PlaceId AS uniqueidentifier
)
RETURNS INT
AS
BEGIN
	DECLARE @AudiosCount AS INT
	SELECT @AudiosCount = COUNT(*)
	  FROM dbo.Audios
	 WHERE Place_Pla_Id = @PlaceId
	RETURN @AudiosCount
END",
@"CREATE FUNCTION [dbo].[StoriesCount]
(
	@PlaceId AS uniqueidentifier
)
RETURNS INT
AS
BEGIN
	DECLARE @StoriesCount AS INT
	SELECT @StoriesCount = COUNT(*)
	  FROM dbo.Stories
	 WHERE Place_Pla_Id = @PlaceId
	RETURN @StoriesCount
END",
@"CREATE FUNCTION [dbo].[AllPlacesId]
(
	@CityId AS INT
) 
RETURNS 
@Place TABLE 
(
	Id UNIQUEIDENTIFIER
)
AS
BEGIN
	INSERT @Place
	SELECT [Pla_Id]
	  FROM [dbo].[Places]
	 WHERE Pla_city_Cit_Id = @CityId AND
		   Pla_Deactive = 0 AND
		   Pla_isOnline = 1
	RETURN 
END
GO",
@"CREATE PROCEDURE [dbo].[GetPackages]
	@CityId int
AS
BEGIN
	DECLARE @Packages TABLE 
	(
		Id uniqueidentifier,
		Name nvarchar(MAX),
		Price bigint,
		PriceD float
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
	SELECT dbo.Packages.Pac_Id, dbo.Packages.Pac_Name, dbo.Packages.Pac_Price, dbo.Packages.Pac_Price_Dollar
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
        public static readonly List<string> ElamSqlCommands = new List<string>()
        {
            @" CREATE TABLE[dbo].[ELMAH_Error]  
                (  
  
                    [ErrorId][uniqueidentifier] NOT NULL,  
  
                    [Application][nvarchar](60) NOT NULL,  
  
                    [Host][nvarchar](50) NOT NULL,  
  
                    [Type][nvarchar](100) NOT NULL,  
  
                    [Source][nvarchar](60) NOT NULL,  
  
                    [Message][nvarchar](500) NOT NULL,  
  
                    [User][nvarchar](50) NOT NULL,  
  
                    [StatusCode][int] NOT NULL,  
  
                    [TimeUtc][datetime] NOT NULL,  
  
                    [Sequence][int] IDENTITY(1, 1) NOT NULL,    
                    [AllXml][ntext] NOT NULL    
                )",
                @"Create PROCEDURE[dbo].[ELMAH_GetErrorsXml]  
  
                (  
                    @Application NVARCHAR(60),  
                    @PageIndex INT = 0,  
                    @PageSize INT = 15,  
                    @TotalCount INT OUTPUT  
  
                )    
                AS    
                SET NOCOUNT ON    
                DECLARE @FirstTimeUTC DATETIME  
                DECLARE @FirstSequence INT  
                DECLARE @StartRow INT  
                DECLARE @StartRowIndex INT  
                SELECT    
                @TotalCount = COUNT(1)   
                FROM    
                    [ELMAH_Error]    
                WHERE    
                    [Application] = @Application  
                SET @StartRowIndex = @PageIndex * @PageSize + 1  
                IF @StartRowIndex <= @TotalCount    
                BEGIN    
                SET ROWCOUNT @StartRowIndex    
                SELECT    
                @FirstTimeUTC = [TimeUtc],    
                    @FirstSequence = [Sequence]   
                FROM    
                    [ELMAH_Error]    
                WHERE    
                    [Application] = @Application   
                ORDER BY    
                    [TimeUtc] DESC,  
                    [Sequence] DESC    
                END    
                ELSE    
                BEGIN    
                SET @PageSize = 0   
                END    
                SET ROWCOUNT @PageSize   
                SELECT    
                errorId = [ErrorId],    
                    application = [Application],  
                    host = [Host],  
                    type = [Type],  
                    source = [Source],  
                    message = [Message],  
                    [user] = [User],  
                    statusCode = [StatusCode],  
                    time = CONVERT(VARCHAR(50), [TimeUtc], 126) + 'Z'  
  
                FROM    
                    [ELMAH_Error] error    
                WHERE    
                    [Application] = @Application  
                AND    
                    [TimeUtc] <= @FirstTimeUTC    
                AND    
                    [Sequence] <= @FirstSequence    
                ORDER BY    
                    [TimeUtc] DESC,   
                    [Sequence] DESC    
                FOR    
                XML AUTO  ",
                @"Create PROCEDURE[dbo].[ELMAH_GetErrorXml]  
  
                (  
  
                    @Application NVARCHAR(60),  
                    @ErrorId UNIQUEIDENTIFIER  
  
                )  
  
                AS  
  
                SET NOCOUNT ON  
                SELECT  
  
                    [AllXml]  
                FROM  
  
                    [ELMAH_Error]  
                WHERE  
  
                    [ErrorId] = @ErrorId  
                AND  
                    [Application] = @Application  ",
                @"Create PROCEDURE[dbo].[ELMAH_LogError]  
                (    
                    @ErrorId UNIQUEIDENTIFIER,    
                    @Application NVARCHAR(60),    
                    @Host NVARCHAR(30),    
                    @Type NVARCHAR(100),  
                    @Source NVARCHAR(60),    
                    @Message NVARCHAR(500),  
                    @User NVARCHAR(50),   
                    @AllXml NTEXT,    
                    @StatusCode INT,   
                    @TimeUtc DATETIME    
                )    
                AS 
                SET NOCOUNT ON    
                INSERT    
                INTO    
                    [ELMAH_Error](    
                    [ErrorId],   
                    [Application],   
                    [Host],  
                    [Type],  
                    [Source],  
                    [Message],    
                    [User],    
                    [AllXml],    
                    [StatusCode],    
                    [TimeUtc]   
                )    
                VALUES  
                    (   
                    @ErrorId,  
                    @Application,    
                    @Host,    
                    @Type,    
                    @Source,   
                    @Message,    
                    @User,   
                    @AllXml,   
                    @StatusCode,   
                    @TimeUtc  
  
                )"
        };
    }
}