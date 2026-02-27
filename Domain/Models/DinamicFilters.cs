using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class DinamicFilters
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public string? SortColumn { get; set; }
        public string? SortOrder { get; set; }
        public string? SearchTerm { get; set; }
        public List<string>? SearchColumns { get; set; } = null;
        public List<string>? Filter { get; set; } = null;
        public List<string>? FilterOperator { get; set; } = null;
        public List<string>? FilterValue { get; set; } = null;
    }
}