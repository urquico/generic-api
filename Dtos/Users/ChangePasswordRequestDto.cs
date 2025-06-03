using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericApi.Dtos.Users
{
    public class ChangePasswordRequestDto
    {
        public required string Password { get; set; } = string.Empty;
        public required string ConfirmPassword { get; set; } = string.Empty;
    }
}
