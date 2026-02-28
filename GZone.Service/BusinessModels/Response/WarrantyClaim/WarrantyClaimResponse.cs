namespace GZone.Service.BusinessModels.Response.WarrantyClaim
{
    public class WarrantyClaimResponse
    {
        public Guid ClaimId { get; set; }
        public string ClaimNumber { get; set; } = null!;
        public string IssueDescription { get; set; } = null!;
        public string ClaimStatus { get; set; } = null!;
        public DateTime ClaimDate { get; set; }
        public DateTime? ResolutionDate { get; set; }
        public string? ResolutionNotes { get; set; }
        public decimal RepairCost { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Status { get; set; } = null!;
        public Guid CustomerId { get; set; }
        public Guid? ProcessedByStaffId { get; set; }
        public Guid OrderDetailId { get; set; }
    }
}
