using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericApi.Dtos.Auth
{
    public class UserJwtDto
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public string FirstName { get; set; } = null!;

        public string? MiddleName { get; set; }

        public string LastName { get; set; } = null!;
        // Add only what you need, no navigation properties
    }
}
