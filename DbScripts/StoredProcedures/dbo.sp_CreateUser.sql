/****** Object:  StoredProcedure [dbo].[sp_CreateUser]    Script Date: 6/3/2025 9:34:32 AM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE PROCEDURE sp_CreateUser
    @Email NVARCHAR(255),
    @HashedPassword NVARCHAR(255),
    @FirstName NVARCHAR(100),
    @MiddleName NVARCHAR(100) = NULL,
    @LastName NVARCHAR(100),
    @StatusId INT = 1,
    @CreatedAt DATETIME,
    @UpdatedAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if email already exists
    IF EXISTS (SELECT 1 FROM Users WHERE Email = @Email)
    BEGIN
        RAISERROR('Email already exists.', 16, 1);
        RETURN;
    END

    -- Insert new user
    INSERT INTO Users (
        Email,
        Password,
        FirstName,
        MiddleName,
        LastName,
        StatusId,
        CreatedAt,
        UpdatedAt
    )
    VALUES (
        @Email,
        @HashedPassword,
        @FirstName,
        @MiddleName,
        @LastName,
        @StatusId,
        @CreatedAt,
        @UpdatedAt
    )
END
GO