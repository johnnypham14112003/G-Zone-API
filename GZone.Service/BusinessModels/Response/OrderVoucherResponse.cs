namespace GZone.Service.BusinessModels.Response
{
    public class OrderVoucherResponse
    {
        public Guid OrderId { get; set; }
        public Guid VoucherId { get; set; }
        public decimal DiscountAmount { get; set; }
        public DateTime AppliedAt { get; set; }

        // Thông tin voucher ðính kèm
        public string VoucherCode { get; set; } = null!;
        public string VoucherName { get; set; } = null!;
        public string DiscountType { get; set; } = null!;
        public decimal DiscountValue { get; set; }

        // Thông tin order ðính kèm
        public string OrderNumber { get; set; } = null!;
        public decimal OrderTotalAmount { get; set; }
        public string OrderStatus { get; set; } = null!;
    }
}
