using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericApi.Dtos.Users
{
    public class ChangePasswordRequestDto
    {
        public string? OldPassword { get; set; } = string.Empty;
        public string? NewPassword { get; set; } = string.Empty;
    }
}
