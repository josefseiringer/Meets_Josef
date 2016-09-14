USE Meets;
GO

create procedure sp_AendereUserdaten
@email varchar(255),
@password varbinary(max)
as
begin
	update Members set password = @password
	where Members.email = @email
end
go


CREATE PROCEDURE sp_holeUserDaten
	@email varchar(255)
AS
BEGIN
	select * from Members
	where Members.email = @email
END
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

CREATE PROCEDURE sp_delete_Member @member INT
AS
BEGIN
	DECLARE @memberval INT = 0;
	SET @memberval = (SELECT mv.id
					  FROM Membervalidation AS mv
					  WHERE mv.member_id = @member);
	IF ( @memberval >= 0)
	BEGIN
		DELETE Membervalidation WHERE id = @memberval
	END;
	IF (@member >= 0)
	BEGIN
		DELETE Members WHERE id = @member
	END;
END;
GO


CREATE PROCEDURE sp_delete_Event 
	@event INT	
AS
BEGIN
	DECLARE @EventIn INT = 0;
	DECLARE	@invi INT = 0;
	SET @EventIn =(SELECT evi.id
				   FROM Eventinvitations AS evi
				   WHERE evi.event_id = @event);
	SET @invi = (SELECT iv.id
		         FROM Invitationstatus AS iv
				 WHERE iv.eventinvitations_id = @EventIn);
	IF (@invi >= 0)
	BEGIN
		DELETE Invitationstatus WHERE id =  @invi
	END;
	IF (@EventIn >= 0)
	BEGIN
		DELETE Eventinvitations WHERE id = @EventIn
	END;
	IF (@event >= 0)
	BEGIN
		DELETE [Events] WHERE id = @event
	END;
END;
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

CREATE FUNCTION [dbo].[fn_detailViewFromUserEmail] (@email VARCHAR(255))
	RETURNS Table	
AS
RETURN(
		SELECT TOP (100) PERCENT CONVERT(VARCHAR(10), 
		dbo.Events.eventdate, 104) AS Eventdatum, 
		dbo.Events.title AS Eventtitel, 
		CONVERT(VARCHAR(10), dbo.Invitationstatus.created, 104) AS Bestätigungszeitpunkt,
		dbo.Eventinvitations.email AS Eingeladen, 
		dbo.Invitationstatus.[confirm] AS Bestätigt, 
		dbo.Members.email
		FROM dbo.Eventinvitations 
		INNER JOIN dbo.Events 
			ON dbo.Eventinvitations.event_id = dbo.Events.id 
		INNER JOIN dbo.Invitationstatus 
			ON dbo.Eventinvitations.id = dbo.Invitationstatus.eventinvitations_id 
		INNER JOIN dbo.Members ON dbo.Events.member_id = dbo.Members.id
		WHERE (dbo.Members.email = @email)
		ORDER BY dbo.Events.eventdate DESC
);

GO


CREATE VIEW [dbo].[BesaetigungDetailView]
AS
SELECT TOP (100) PERCENT CONVERT(VARCHAR(10), dbo.Events.eventdate, 104) AS Eventdatum, dbo.Events.title AS Eventtitel, CONVERT(VARCHAR(10), dbo.Invitationstatus.created, 104) AS Bestätigungszeitpunkt, dbo.Eventinvitations.email AS Eingeladen, 
             dbo.Invitationstatus.[confirm] AS Bestätigt
FROM   dbo.Eventinvitations INNER JOIN
             dbo.Events ON dbo.Eventinvitations.event_id = dbo.Events.id INNER JOIN
             dbo.Invitationstatus ON dbo.Eventinvitations.id = dbo.Invitationstatus.eventinvitations_id INNER JOIN
             dbo.Members ON dbo.Events.member_id = dbo.Members.id
ORDER BY dbo.Events.eventdate DESC;

GO