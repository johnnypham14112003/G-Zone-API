namespace GZone.Service.BusinessModels.Request.Order
{
    public class OrderQuery
    {
        // ===== Search =====
        public string? SearchTerm { get; set; } // Tìm theo OrderNumber, ReceiverName, ReceiverPhone

        // ===== Filters =====
        public string? Status { get; set; }
        public string? PaymentStatus { get; set; }
        public string? PaymentMethod { get; set; }
        public Guid? CustomerId { get; set; }
        public bool? WholeSale { get; set; }

        // ===== Date Range =====
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        // ===== Sorting =====
        // newest | oldest | total_asc | total_desc
        public string? SortBy { get; set; }
    }
}
