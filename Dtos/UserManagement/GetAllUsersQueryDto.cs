using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericApi.Dtos;

namespace GenericApi.Dtos.UserManagement
{
    public class GetAllUsersQueryDto : PaginationDto
    {
        public string? role { get; set; } = null;
    }
}
