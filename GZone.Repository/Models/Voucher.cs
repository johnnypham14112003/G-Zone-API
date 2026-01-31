using System.ComponentModel.DataAnnotations;

namespace GZone.Repository.Models
{
    public class Voucher
    {
        [Key]
        public Guid VoucherId { get; set; }
        public string VoucherCode { get; set; }
        public string VoucherName { get; set; }
        public string Description { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal MaxDiscountAmount { get; set; }
        public int MinOrderAmount { get; set; }
        public int MaxUsageTotal { get; set; }
        public int MaxUsagePerCustomer { get; set; }
        public int CurrentUsageCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ApplicableTo { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        // Foreign keys
        public Guid CreatedByStaffId { get; set; }

        // Navigation properties
        public virtual Account Staff { get; set; } = null!;
        public virtual ICollection<OrderVoucher>? OrderVouchers { get; set; }
        public virtual ICollection<UserVoucher>? UserVouchers { get; set; }
    }
}
