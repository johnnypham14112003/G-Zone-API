using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZone.Service.BusinessModels.Response.Customization
{
    public class CustomizationResponse
    {
        public Guid CustomId { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public decimal QuotedPrice { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CustomerId { get; set; }
        public Guid ProductId { get; set; }
        public string CustomerName { get; set; }
        public string? StaffName { get; set; }
        public string ProductName { get; set; }
    }

}
