/****** Object:  StoredProcedure [dbo].[sp_RevokeRefreshToken]    Script Date: 6/4/2025 3:21:57 PM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE PROCEDURE sp_RevokeRefreshToken
    @Token NVARCHAR(512),
    @RevokedBy NVARCHAR(128)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE RefreshTokens
    SET RevokedAt = GETUTCDATE(),
        RevokedBy = @RevokedBy
    WHERE Token = @Token
END
GO