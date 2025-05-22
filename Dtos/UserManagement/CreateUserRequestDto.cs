using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericApi.Dtos.UserManagement
{
    public class CreateUserRequestDto
    {
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
    }
}
