SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE VIEW vw_AllPermissions AS
SELECT 
    p.Id,
    p.permission_name,
    CASE WHEN p.permission_status = 1 THEN 'Active' ELSE 'Inactive' END AS PermissionStatus,
    ISNULL(m.module_name, 'No Module') AS ModuleName
FROM fmis.module_permissions p
LEFT JOIN fmis.modules m ON p.module_id = m.Id