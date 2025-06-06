using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericApi.Dtos.UserManagement;

namespace GenericApi.Dtos.Auth
{
    public class UserJwtDto : MinimalUserDto
    {
        public List<Role> Roles { get; set; } = [];
        public List<Permission> Permissions { get; set; } = [];

        // Add only what you need, no navigation properties
    }

    public class MinimalUserDto
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public string FirstName { get; set; } = null!;

        public string? MiddleName { get; set; }

        public string LastName { get; set; } = null!;

        // Add only what you need, no navigation properties
    }
}
