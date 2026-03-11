namespace GZone.Service.BusinessModels.Request.PaymentTransaction
{
    public class PaymentTransactionRequest
    {
        public Guid OrderId { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentGateway { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
    }
}
