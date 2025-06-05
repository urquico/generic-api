/****** Object:  StoredProcedure [dbo].[sp_UserCreate]    Script Date: 6/4/2025 4:32:15 PM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE PROCEDURE [dbo].[sp_UserCreate]
    @Email NVARCHAR(255),
    @HashedPassword NVARCHAR(255),
    @FirstName NVARCHAR(100),
    @MiddleName NVARCHAR(100) = NULL,
    @LastName NVARCHAR(100),
    @StatusId INT = 1,
    @CreatedAt DATETIME,
    @CreatedBy NVARCHAR(255),
    @UpdatedAt DATETIME,
    @UpdatedBy NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if email already exists
    IF EXISTS (SELECT 1 FROM fmis.users u WHERE u.email= @Email)
    BEGIN
        RAISERROR('Email already exists.', 16, 1);
        RETURN;
    END

    -- Insert new user
    INSERT INTO fmis.users (
        email,
        password,
        first_name,
        middle_name,
        last_name,
        status_id,
        created_at,
        created_by,
        updated_at,
        updated_by
    )
    VALUES (
        @Email,
        @HashedPassword,
        @FirstName,
        @MiddleName,
        @LastName,
        @StatusId,
        @CreatedAt,
        @CreatedBy,
        @UpdatedAt,
        @UpdatedBy
    );
END
GO