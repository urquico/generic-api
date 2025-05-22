using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericApi.Dtos.Users
{
    public class UpdateUserInfoRequestDto
    {
        public string? FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
    }
}
