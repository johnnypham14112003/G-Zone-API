using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GZone.Repository.Models
{
    public class Order
    {
        [Key]
        public Guid OrderId { get; set; } = Guid.NewGuid();
        public string OrderNumber { get; set; }
        public bool WholeSale { get; set; }
        public string Status { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime? PaymentAt { get; set; }

        public string ShippingAddress { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingDistrict { get; set; }
        public string ShippingWard { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }

        public string Note { get; set; }
        public string TrackingNumber { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string CancellationReason { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Foreign keys
        public Guid CustomerId { get; set; }
        public Guid? ManagedByStaffId { get; set; }

        // Navigation properties
        public virtual Account Customer { get; set; } = null!;
        public virtual Account? Staff { get; set; }
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
        public virtual ICollection<PaymentTransaction>? Transactions { get; set; }
        public virtual ICollection<OrderVoucher>? OrderVouchers { get; set; }
        public virtual ICollection<Notification>? Notifications { get; set; }
    }
}
