namespace GZone.Service.BusinessModels.Request.PaymentTransaction
{
    public class PaymentTransactionPatchRequest
    {
        public string? Status { get; set; }
        public string? GatewayResponse { get; set; }
        public Guid? VerifiedByStaffId { get; set; }
    }
}
