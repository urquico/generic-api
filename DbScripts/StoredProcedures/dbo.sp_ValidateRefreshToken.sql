/****** Object:  StoredProcedure [dbo].[sp_ValidateRefreshToken]    Script Date: 6/4/2025 3:22:11 PM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

CREATE PROCEDURE sp_ValidateRefreshToken
    @RefreshToken NVARCHAR(512)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        rt.Id AS RefreshTokenId,
        rt.UserId,
        rt.ExpiresAt,
        rt.RevokedAt,
        u.Id AS UserId,
        u.Email,
        u.FirstName,
        u.MiddleName,
        u.LastName
    FROM RefreshTokens rt
    INNER JOIN Users u ON u.Id = rt.UserId
    WHERE rt.Token = @RefreshToken
END
GO