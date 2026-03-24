using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZone.Service.BusinessModels.Request.Customization
{
    public class CustomizationUpdateRequest
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public decimal Weight { get; set; }
        public decimal QuotedPrice { get; set; }
        public string StaffNote { get; set; }
        public string Status { get; set; }
        public Guid? ConfirmedByStaffId { get; set; }
    }
}
