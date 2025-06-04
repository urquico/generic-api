using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericApi.Dtos.RolesManagement
{
    public class GetAllRolesResponseDto
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = null!;
        public string Status { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new List<string>();
        public String DateCreated { get; set; } = null!;
    }
}
