using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZone.Service.BusinessModels.Request.Customization
{
    public class CustomizationQuery
    {
        public string? Keyword { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? ProductId { get; set; }
        public string? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SortBy { get; set; }
    }

}
