--Hash passwort
CREATE FUNCTION fn_hash_password(@text_to_hash VARBINARY(MAX) )
	RETURNS VARBINARY(max)
AS
BEGIN
	RETURN HASHBYTES('sha2_512',@text_to_hash); 
END;
GO
--chek user
CREATE FUNCTION fn_check_user_scalar( @email NVARCHAR(80), @password VARBINARY(MAX), @geburtstag DATETIME)
	RETURNS INT
AS
BEGIN
	DECLARE @result INT = NULL;
	SELECT TOP 1 @result = u.id
	FROM [User] AS u
	WHERE u.email = @email
	AND u.passwort = dbo.fn_hash_password(@password)
	AND u.geburtstag = @geburtstag;
	
	RETURN @result;
END;
GO