namespace GZone.Service.BusinessModels.Response
{
    public class VoucherResponse
    {
        public Guid VoucherId { get; set; }
        public string VoucherCode { get; set; } = null!;
        public string VoucherName { get; set; } = null!;
        public string? Description { get; set; }
        public string DiscountType { get; set; } = null!;
        public decimal DiscountValue { get; set; }
        public decimal MaxDiscountAmount { get; set; }
        public int MinOrderAmount { get; set; }
        public int MaxUsageTotal { get; set; }
        public int MaxUsagePerCustomer { get; set; }
        public int CurrentUsageCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? ApplicableTo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public Guid CreatedByStaffId { get; set; }
    }
}
