namespace GZone.Service.BusinessModels.Request.WarrantyClaim
{
    public class WarrantyClaimRequest
    {
        public string IssueDescription { get; set; } = null!;
        public Guid CustomerId { get; set; }
        public Guid OrderDetailId { get; set; }
    }
}
