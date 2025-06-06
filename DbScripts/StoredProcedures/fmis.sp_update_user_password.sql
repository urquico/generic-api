/****** Object:  StoredProcedure [fmis].[sp_update_user_password]    Script Date: 6/6/2025 2:13:30 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [fmis].[sp_update_user_password]
    @UserId INT,
    @NewPassword NVARCHAR(255),
    @ConfirmPassword NVARCHAR(255),
    @HashedPassword NVARCHAR(255),
    @UpdatedBy INT,
    @StatusCode INT OUTPUT,
    @Message NVARCHAR(255) OUTPUT,
    @Data NVARCHAR(MAX) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- 404 Not Found: User does not exist or is deleted
        IF NOT EXISTS (
            SELECT 1 FROM [fmis].[users]
            WHERE id = @UserId AND deleted_at IS NULL
        )
        BEGIN
            SET @StatusCode = 404;
            SET @Message = 'User not found.';
            SET @Data = NULL;
            RETURN;
        END

        -- 400 Bad Request: Passwords do not match
        IF @NewPassword != @ConfirmPassword
        BEGIN
            SET @StatusCode = 400;
            SET @Message = 'Passwords do not match.';
            SET @Data = NULL;
            RETURN;
        END

        -- Update password
        UPDATE [fmis].[users]
        SET [password] = @HashedPassword,
            [updated_at] = GETUTCDATE(),
            [updated_by] = @UpdatedBy
        WHERE id = @UserId;        

        SET @StatusCode = 200;
        SET @Message = 'Password updated successfully.';
        SET @Data = NULL;
    END TRY
    BEGIN CATCH
        SET @StatusCode = 500;
        SET @Message = ERROR_MESSAGE();
        SET @Data = NULL;
    END CATCH
END

GO
