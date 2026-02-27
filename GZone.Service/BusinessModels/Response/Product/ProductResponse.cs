using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZone.Service.BusinessModels.Response.Product
{
    public class ProductResponse
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string Sku { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public string Brand { get; set; }
        public string Material { get; set; }
        public string Specifications { get; set; }
        public decimal Weight { get; set; }
        public string Dimension { get; set; }
        public int ViewCount { get; set; }
        public int SoldCount { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public int WarrantyPeriodMonths { get; set; }

        // Foreign keys
        public Guid CategoryId { get; set; }
    }
}
