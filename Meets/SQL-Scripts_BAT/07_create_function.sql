USE Meets;
GO

-- Zeigt die 5 Benutzer an, die am öftesten auf eine Einladung positiv geantwortet haben
--  (Angenommene Einladungen).

CREATE PROCEDURE sp_Top5UserRanking
AS
BEGIN

	SELECT TOP 5 evi.email, 
		   COUNT(ev.id) AS confirms 
	FROM [Events] AS ev
		JOIN Eventinvitations as evi
			ON ev.id = evi.event_id
		JOIN Invitationstatus AS ivs
			ON evi.id = ivs.eventinvitations_id					
WHERE ivs.confirm = 1
GROUP BY evi.email
ORDER BY Confirms DESC

END
GO

-- Meine Gesamtanzahl an Einladungen die ich bekommen habe (Erhaltene Einladungen).

CREATE PROCEDURE sp_Eventinvitations
	@email varchar(255)

AS
BEGIN
	SELECT evi.email, COUNT(evi.id) AS invitations FROM Eventinvitations AS evi	
WHERE evi.email = @email
GROUP BY evi.email

END
GO

-- Anzahl der "Erstellten Einladungen", Anzeige ob valid und Anzeige der zugehörigen E-Mail-Adresse

CREATE PROCEDURE sp_UserInvitations	
AS
BEGIN

	SELECT Members.id, 
		   Members.email, 
		   COUNT(Eventinvitations.id) AS invitations,
		   COUNT(Membervalidation.id) AS valid 
	FROM Members
		LEFT JOIN Membervalidation 
			ON Members.id = Membervalidation.member_id
		LEFT JOIN [Events] 
			ON Members.id = [Events].member_id
		LEFT JOIN Eventinvitations 
			ON [Events].id = Eventinvitations.event_id	
	GROUP BY Members.id, Members.email
END
GO

-- gibt einen int zurück wieviele haben zugesagt
CREATE PROCEDURE sp_GetConfirms
	@eventId int
AS
BEGIN
	select count(confirm) as zusagen from [Events]
	join Eventinvitations
	on [Events].id = Eventinvitations.event_id
	join Invitationstatus
	on Eventinvitations.id = Invitationstatus.eventinvitations_id
	where [Events].id = @eventId and confirm = 1;
END
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
		CONVERT(VARCHAR(10), dbo.Invitationstatus.created, 104) AS Bestaetigungszeitpunkt,
		dbo.Eventinvitations.email AS Eingeladen, 
		dbo.Invitationstatus.[confirm] AS Bestaetigt, 
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
SELECT TOP (100) PERCENT CONVERT(VARCHAR(10), dbo.Events.eventdate, 104) AS Eventdatum, dbo.Events.title AS Eventtitel, CONVERT(VARCHAR(10), dbo.Invitationstatus.created, 104) AS Bestaetigungszeitpunkt, dbo.Eventinvitations.email AS Eingeladen, 
             dbo.Invitationstatus.[confirm] AS Bestaetigt
FROM   dbo.Eventinvitations INNER JOIN
             dbo.Events ON dbo.Eventinvitations.event_id = dbo.Events.id INNER JOIN
             dbo.Invitationstatus ON dbo.Eventinvitations.id = dbo.Invitationstatus.eventinvitations_id INNER JOIN
             dbo.Members ON dbo.Events.member_id = dbo.Members.id
ORDER BY dbo.Events.eventdate DESC;
GO

-- Funktion zählt wieviele Events ein User schon angelegt hat
CREATE FUNCTION fn_TopUsers()
RETURNS TABLE 
AS
RETURN 
(
	select me.email AS EMail,
		   Count(ev.member_id) AS EventCount
	from Events as ev
	join Members as me
		on me.id = ev.member_id
	Group by me.email
);
GO

--Prozedur ruft Funktion auf wieviele Events pro User vom meisten zum wenigsten
CREATE PROCEDURE sp_Call_TopUser
AS
BEGIN
    -- Insert statements for procedure here
	select * from fn_TopUsers() ORDER BY EventCount DESC
END
GO