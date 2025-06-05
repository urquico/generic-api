SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE VIEW vw_GetAllUsers AS
SELECT 
    u.Id,
    u.Email,
    u.first_name,
    u.last_name,
    u.created_at,
    u.created_by,
    s.category_value AS Status,
    r.role_name
FROM fmis.users u
LEFT JOIN mwss.key_categories s ON u.status_id= s.Id
LEFT JOIN fmis.user_roles ur ON ur.user_id = u.Id
LEFT JOIN fmis.roles r ON ur.role_id = r.Id;