namespace GZone.Service.BusinessModels.Response
{
    public class UserVoucherResponse
    {
        public Guid AccountId { get; set; }
        public Guid VoucherId { get; set; }
        public bool IsUsed { get; set; }

        // Thông tin voucher ðính kèm
        public string VoucherCode { get; set; } = null!;
        public string VoucherName { get; set; } = null!;
        public string DiscountType { get; set; } = null!;
        public decimal DiscountValue { get; set; }
        public decimal MaxDiscountAmount { get; set; }
        public int MinOrderAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
