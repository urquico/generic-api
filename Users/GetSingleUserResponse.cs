using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScaffoldTest.Dtos.Users
{
    public class GetSingleUserResponse
    {
        public int Id { get; set; } = 0;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Birthday { get; set; }
        public string? Created_at { get; set; }
        public bool? Is_active { get; set; } = false;
    }
}
