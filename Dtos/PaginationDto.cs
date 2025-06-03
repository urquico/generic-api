using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericApi.Dtos
{
    public class PaginationDto
    {
        public int? page { get; set; } = 1;
        public int? limit { get; set; } = 10;
        public int? includeDeleted { get; set; } = 0;
        public string? status { get; set; } = null;
        public string? searchType { get; set; } = null;
        public string? searchValue { get; set; } = null;
        public DateTime? from { get; set; } = null;
        public DateTime? to { get; set; } = null;
    }
}
