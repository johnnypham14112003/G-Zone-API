namespace GZone.Service.BusinessModels.Response.PaymentTransaction
{
    public class PaymentTransactionResponse
    {
        public Guid TransactionId { get; set; }
        public string TransactionNumber { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentGateway { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public string GatewayResponse { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime CreatedAt { get; set; }

        // Foreign keys
        public Guid OrderId { get; set; }
        public Guid VerifiedByStaffId { get; set; }
    }
}
