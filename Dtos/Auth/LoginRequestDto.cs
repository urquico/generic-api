using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericApi.Dtos.Auth
{
    public class LoginRequestDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
