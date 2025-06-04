/****** Object:  StoredProcedure [dbo].[sp_UserLogin]    Script Date: 6/4/2025 4:32:15 PM ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE PROCEDURE [dbo].[sp_UserLogin]
    @Email NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        u.id,
        u.email,
        u.password,
        u.first_name,
        u.middle_name,
        u.last_name,
        u.status_id,
        u.created_at,
        u.created_by
    FROM
        fmis.users u
    WHERE
        u.email = @Email
END
GO