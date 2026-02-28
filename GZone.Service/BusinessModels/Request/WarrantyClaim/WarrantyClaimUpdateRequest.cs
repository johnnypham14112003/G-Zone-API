namespace GZone.Service.BusinessModels.Request.WarrantyClaim
{
    public class WarrantyClaimUpdateRequest
    {
        public string? ClaimStatus { get; set; }
        public string? ResolutionNotes { get; set; }
        public decimal? RepairCost { get; set; }
        public Guid? ProcessedByStaffId { get; set; }
        public string? Status { get; set; }
    }
}
