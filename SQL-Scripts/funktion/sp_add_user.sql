CREATE PROCEDURE sp_add_user
	@email NVARCHAR(80),
	@password VARBINARY(MAX),
	@geburtstag DATETIME
AS
BEGIN
	DECLARE @result INT = NULL;
	DECLARE @user_id INT = NULL;
	DECLARE @user_units INT = NULL;

	SELECT @user_units = COUNT(u.id) 
	FROM [User] AS u
	
	SET @user_id = dbo.fn_hash_password(@password);
	
	IF(@user_units < 2) AND (@result IS NULL) AND (@user_id IS NULL)
	BEGIN
		SET @result = 1;
	END;
	IF(@result = 1)
	BEGIN
		INSERT INTO [User](email, passwort, geburtstag)
		VALUES(@email , dbo.fn_hash_password(@password),@geburtstag )
	END;

END
GO
--DECLARE @HashThis varbinary(max)
--SET @HashThis = CONVERT(varbinary(max),'123user!')  
--exec dbo.sp_add_user 'j.seiringer@live.at',@HashThis, '2016-25-07'