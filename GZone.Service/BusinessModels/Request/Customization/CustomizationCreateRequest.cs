using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZone.Service.BusinessModels.Request.Customization
{
    public class CustomizationCreateRequest
    {
        public string Name { get; set; }
        public string Sku { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public decimal Weight { get; set; }
        public string StaffNote { get; set; }

        public Guid CustomerId { get; set; }
        public Guid ProductId { get; set; }
    }

}
