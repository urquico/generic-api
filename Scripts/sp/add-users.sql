-- Create stored procedure
CREATE PROCEDURE mwss.AddUser
    @Username NVARCHAR(100),
    @Email NVARCHAR(255)
AS
BEGIN
    INSERT INTO mwss.Users (Username, Email)
    VALUES (@Username, @Email);

    -- return the created user
    SELECT * FROM mwss.vw_UserByUsernameEmail WHERE Username = @Username AND Email = @Email;
END;