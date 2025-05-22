using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericApi.Dtos.UserManagement
{
    public class UpdateUserStatusRequestDto
    {
        public string Status { get; set; } = string.Empty; // e.g., "Active", "Inactive", "Suspended"
    }
}
