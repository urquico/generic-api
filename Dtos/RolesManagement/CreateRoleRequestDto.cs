using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericApi.Dtos.RolesManagement
{
    public class CreateRoleRequestDto
    {
        public string RoleName { get; set; } = string.Empty;

        public List<int> Permissions { get; set; } = [];
    }
}
