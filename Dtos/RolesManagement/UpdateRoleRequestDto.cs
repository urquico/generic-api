using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericApi.Dtos.RolesManagement
{
    public class UpdateRoleRequestDto
    {
        public required string RoleName { get; set; }
        public bool RoleStatus { get; set; }
        public List<int> Permissions { get; set; } = [];
    }
}
