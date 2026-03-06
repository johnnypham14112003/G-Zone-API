namespace GZone.Service.BusinessModels.Request.PaymentTransaction
{
    public class PaymentTransactionQuery
    {
        // ===== Search =====
        public string? SearchTerm { get; set; } // Tìm theo TransactionNumber

        // ===== Filters =====
        public Guid? OrderId { get; set; }
        public string? Status { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentGateway { get; set; }

        // ===== Date Range =====
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        // ===== Sorting =====
        // newest | oldest | amount_asc | amount_desc
        public string? SortBy { get; set; }
    }
}
