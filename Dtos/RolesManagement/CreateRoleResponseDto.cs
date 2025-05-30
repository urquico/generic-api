using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericApi.Dtos.RolesManagement
{
    public class CreateRoleResponseDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;

        public List<CreatedPermissionDto> Permissions { get; set; } = [];
    }

    public class CreatedPermissionDto
    {
        public int PermissionId { get; set; }
        public string PermissionName { get; set; } = string.Empty;
    }
}
