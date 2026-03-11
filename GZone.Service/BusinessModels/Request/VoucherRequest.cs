using System.ComponentModel.DataAnnotations;

namespace GZone.Service.BusinessModels.Request
{
    public class VoucherRequest
    {
        public Guid? VoucherId { get; set; }

        [Required]
        [MaxLength(50)]
        public string VoucherCode { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string VoucherName { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        [MaxLength(30)]
        public string DiscountType { get; set; } = null!;  // e.g. "Percentage", "Fixed"

        [Range(0, double.MaxValue)]
        public decimal DiscountValue { get; set; }

        [Range(0, double.MaxValue)]
        public decimal MaxDiscountAmount { get; set; }

        [Range(0, int.MaxValue)]
        public int MinOrderAmount { get; set; }

        [Range(0, int.MaxValue)]
        public int MaxUsageTotal { get; set; }

        [Range(0, int.MaxValue)]
        public int MaxUsagePerCustomer { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [MaxLength(50)]
        public string? ApplicableTo { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
