/****** Object:  StoredProcedure [dbo].[sp_UserLogin]    Script Date: 6/4/2025 3:22:07 PM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE PROCEDURE sp_UserLogin
    @Email NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        Id,
        Email,
        Password,
        FirstName,
        MiddleName,
        LastName,
        StatusId,
        CreatedAt,
        UpdatedAt
    FROM
        Users
    WHERE
        Email = @Email
END
GO