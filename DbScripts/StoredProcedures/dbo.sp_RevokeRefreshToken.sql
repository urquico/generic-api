/****** Object:  StoredProcedure [dbo].[sp_RevokeRefreshToken]    Script Date: 6/4/2025 4:32:15 PM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE PROCEDURE [dbo].[sp_RevokeRefreshToken]
    @Token NVARCHAR(512),
    @RevokedBy NVARCHAR(128)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE fmis.refresh_tokens 
    SET revoked_at = GETUTCDATE(),
        revoked_by = @RevokedBy
    WHERE token = @Token
END
GO