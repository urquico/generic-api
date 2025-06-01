using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericApi.Dtos.UserManagement
{
    public class UpdateUserStatusRequestDto
    {
        public required int Status { get; set; }
    }
}
