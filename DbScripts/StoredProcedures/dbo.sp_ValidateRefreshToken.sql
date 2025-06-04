/****** Object:  StoredProcedure [dbo].[sp_ValidateRefreshToken]    Script Date: 6/4/2025 4:32:15 PM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE PROCEDURE [dbo].[sp_ValidateRefreshToken]
    @RefreshToken NVARCHAR(512)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        rt.Id AS RefreshTokenId,
        rt.user_id,
        rt.expires_at,
        rt.revoked_at,
        u.id AS UserId,
        u.email,
        u.first_name,
        u.middle_name,
        u.last_name
    FROM fmis.refresh_tokens rt
    INNER JOIN fmis.users u ON u.id = rt.user_id
    WHERE rt.token= @RefreshToken
END
GO