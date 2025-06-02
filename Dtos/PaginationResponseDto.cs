using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericApi.Dtos
{
    public class PaginationResponseDto<T>
    {
        public List<T> Items { get; set; } = [];
        public int TotalCount { get; set; } = 0;
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
    }
}
