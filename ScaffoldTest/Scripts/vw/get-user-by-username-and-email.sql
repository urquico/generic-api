CREATE VIEW mwss.vw_UserByUsernameEmail AS
SELECT
    ID,
    Username,
    Email,
    Birthday,
    CreatedAt,
    IsActive
FROM mwss.Users