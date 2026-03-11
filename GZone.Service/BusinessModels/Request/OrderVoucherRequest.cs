namespace GZone.Service.BusinessModels.Request
{
    public class OrderVoucherRequest
    {
        public Guid OrderId { get; set; }
        public Guid VoucherId { get; set; }
    }
}
