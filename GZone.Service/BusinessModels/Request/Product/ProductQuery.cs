using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZone.Service.BusinessModels.Request.Product
{
    public class ProductQuery
    {
        // ===== Search =====
        public string? Keyword { get; set; }

        // ===== Filters =====
        public Guid? CategoryId { get; set; }
        public string? Brand { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public bool? IsActive { get; set; }
        public bool? IsFeatured { get; set; }

        // ===== Sorting =====
        // price_asc | price_desc | newest | oldest | popular
        public string? SortBy { get; set; }
    }
}
