using System.ComponentModel.DataAnnotations;

namespace GZone.Repository.Models
{
    public class PaymentTransaction
    {
        [Key]
        public Guid TransactionId { get; set; }
        public string TransactionNumber { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentGateway { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public string GatewayResponse { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;


        // Foreign keys
        public Guid OrderId { get; set; }
        public Guid VerifiedByStaffId { get; set; }

        // Navigation properties
        public virtual Order Order { get; set; } = null!;
        public virtual Account Staff { get; set; } = null!;
    }
}
