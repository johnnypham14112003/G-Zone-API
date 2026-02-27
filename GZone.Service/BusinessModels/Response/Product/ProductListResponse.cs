using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZone.Service.BusinessModels.Response.Product
{
    public class ProductListResponse
    {
        // ===== Paging Info =====
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages =>
            PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);

        // ===== Data =====
        public List<ProductResponse> Items { get; set; } = new();
    }
}

