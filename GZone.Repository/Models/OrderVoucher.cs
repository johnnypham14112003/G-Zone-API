using System.ComponentModel.DataAnnotations;

namespace GZone.Repository.Models
{
    public class OrderVoucher
    {
        public decimal DiscountAmount { get; set; }
        public DateTime AppliedAt { get; set; } = DateTime.Now;

        // Foreign keys
        public Guid OrderId { get; set; }
        public Guid VoucherId { get; set; }

        // Navigation properties
        public virtual Order Order { get; set; } = null!;
        public virtual Voucher Voucher { get; set; } = null!;
    }
}
