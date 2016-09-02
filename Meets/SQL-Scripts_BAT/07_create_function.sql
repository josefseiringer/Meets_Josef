USE Meets;
GO


CREATE PROCEDURE sp_RegisterUser
	@email Varchar(255),
	@password varbinary(MAX),
	@dateOfBirth datetime
AS
BEGIN
	insert into Members(email,[password],dateofbirth) values(@email,@password,@dateOfBirth);
END
GO

CREATE FUNCTION fn_check_user_Table( @email VARCHAR(255), @password VARBINARY(MAX))
	RETURNS @retTable TABLE(id INT NOT NULL)
AS
BEGIN
	INSERT INTO @retTable(id)
	SELECT TOP 1 m.id
	FROM [Members] AS m
	WHERE m.email = @email
	AND m.[password] = dbo.fn_hash_password(@password);
	
	RETURN;
END
GO

CREATE FUNCTION fn_check_user_scalar( @email VARCHAR(255), @password VARBINARY(MAX))
	RETURNS INT
AS
BEGIN
	DECLARE @result INT = 0;
	SELECT TOP 1 @result = m.id
	FROM [Members] AS m
	WHERE m.email = @email
	AND m.[password] = dbo.fn_hash_password(@password);
	
	RETURN @result;
END
GO

CREATE FUNCTION [dbo].[fn_Check_User_FromEmail]( @email VARCHAR(255))
	RETURNS INT
AS
BEGIN
	DECLARE @result INT = 0;
	SELECT TOP 1 @result = m.id
	FROM [Members] AS m
	WHERE m.email = @email;
	
	RETURN @result;
END;
GO

CREATE FUNCTION fn_Show_Event_Table(@email varchar(255))
	RETURNS @retTable TABLE(id int,evdate DATETIME,title varchar(50),evdesc varchar(255),loc varchar(255),pub BIT)
AS
BEGIN
	DECLARE @user_id INT = NULL;
	SET @user_id = dbo.fn_Check_User_FromEmail(@email)
	IF (NOT @user_id IS NULL) AND (@user_id > 0)
		BEGIN
		INSERT INTO @retTable(id,evdate,title,evdesc,loc,pub)
			select ev.id,ev.eventdate,ev.title,ev.[description],ev.location,ev.viewpublic
			from Meets.dbo.Members as me 
			JOIN Meets.dbo.[Events] as ev ON ev.member_id = me.id
			JOIN Meets.dbo.Membervalidation as mv ON me.id = mv.member_id
			where me.id = 1;
	END
RETURN;
END
GO



CREATE FUNCTION fn_hash_password(@text_to_hash VARBINARY(MAX) )
	RETURNS VARBINARY(MAX)
AS
BEGIN
	RETURN HASHBYTES('sha2_512',@text_to_hash); 
END;
GO

CREATE VIEW View_Event
AS
SELECT dbo.[Events].created AS Erstellt, dbo.Members.email AS Ersteller, dbo.[Events].eventdate AS Termin, dbo.[Events].title AS Veranstaltungsname, 
       dbo.[Events].description AS Beschreibung, dbo.[Events].viewpublic AS Oeffentlich, dbo.[Events].location AS Standort
FROM   dbo.[Events] INNER JOIN dbo.Members ON dbo.[Events].member_id = dbo.Members.id;
GO

CREATE VIEW View_Event_open
AS
SELECT dbo.[Events].created AS Erstellt, dbo.Members.email AS Ersteller, dbo.[Events].eventdate AS Termin, dbo.[Events].title AS Veranstaltungsname, 
       dbo.[Events].description AS Beschreibung, dbo.[Events].viewpublic AS Oeffentlich, dbo.[Events].location AS Standort
FROM   dbo.[Events] INNER JOIN dbo.Members ON dbo.[Events].member_id = dbo.Members.id where dbo.[Events].viewpublic = 1;
GO
