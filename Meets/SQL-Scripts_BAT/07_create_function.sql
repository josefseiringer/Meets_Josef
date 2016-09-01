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
