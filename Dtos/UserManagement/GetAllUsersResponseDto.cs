using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericApi.Dtos.UserManagement
{
    public class GetAllUsersResponseDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string[] Roles { get; set; } = null!;
        public string DateCreated { get; set; } = null!;
        public string LastLogin { get; set; } = null!;
    }
}
