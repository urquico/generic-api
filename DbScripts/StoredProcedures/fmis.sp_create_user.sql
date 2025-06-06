/****** Object:  StoredProcedure [fmis].[sp_create_user]    Script Date: 6/6/2025 2:13:30 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [fmis].[sp_create_user]
    @Email NVARCHAR(255),
    @Password NVARCHAR(255),
    @ConfirmPassword NVARCHAR(255),
    @HashedPassword NVARCHAR(255),
    @FirstName NVARCHAR(100),
    @MiddleName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @CreatedBy INT,
    @UserRoleIds NVARCHAR(MAX),
    @StatusCode INT OUTPUT,
    @Message NVARCHAR(255) OUTPUT,
    @Data NVARCHAR(MAX) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- 409 Conflict: Email already exists
        IF EXISTS (
            SELECT 1 
            FROM [fmis].[users] 
            WHERE email = @Email AND deleted_at IS NULL
        )
        BEGIN
            SET @StatusCode = 409;
            SET @Message = 'Email already exists.';
            SET @Data = NULL;
            RETURN;
        END

        -- 400 Bad Request: Passwords do not match
        IF @Password != @ConfirmPassword
        BEGIN
            SET @StatusCode = 400;
            SET @Message = 'Passwords do not match.';
            SET @Data = NULL;
            RETURN;
        END

        DECLARE @NowUTC DATETIME2 = GETUTCDATE();

        -- Insert new user
        INSERT INTO [fmis].[users] (
            [email], [password], [first_name], [middle_name], [last_name],
            [status_id], [created_at], [created_by], [updated_at], [updated_by]
        )
        VALUES (
            @Email, @HashedPassword, @FirstName, @MiddleName, @LastName,
            1, @NowUTC, @CreatedBy, @NowUTC, @CreatedBy
        );

        DECLARE @NewUserId INT = SCOPE_IDENTITY();

        -- Insert roles if provided
        IF LTRIM(RTRIM(ISNULL(@UserRoleIds, ''))) != ''
        BEGIN
            INSERT INTO [fmis].[user_roles] (
                [user_id], [role_id], [created_at], [created_by], [updated_at], [updated_by]
            )
            SELECT 
                @NewUserId, 
                TRY_CAST(value AS INT), 
                @NowUTC, @CreatedBy,
                @NowUTC, @CreatedBy
            FROM STRING_SPLIT(@UserRoleIds, ',')
            WHERE TRY_CAST(value AS INT) IS NOT NULL;
        END

        -- Return user info as JSON
        SELECT @Data = (
            SELECT 
                id,
                email,
                first_name,
                middle_name,
                last_name,
                created_at,
                created_by
            FROM [fmis].[users]
            WHERE id = @NewUserId
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        );

        SET @StatusCode = 200;
        SET @Message = 'User created successfully.';
    END TRY
    BEGIN CATCH
        SET @StatusCode = 500;
        SET @Message = ERROR_MESSAGE();
        SET @Data = NULL;
    END CATCH
END

GO
