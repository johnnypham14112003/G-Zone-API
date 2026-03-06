namespace GZone.Service.BusinessModels.Request
{
    public class UserVoucherRequest
    {
        public Guid AccountId { get; set; }
        public Guid VoucherId { get; set; }
    }
}
