namespace GZone.Service.BusinessModels.Request.WarrantyClaim
{
    public class WarrantyClaimQuery
    {
        public string? Keyword { get; set; }
        public string? ClaimStatus { get; set; }
        public string? Status { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? ProcessedByStaffId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SortBy { get; set; } // newest | oldest | cost_asc | cost_desc
    }
}
