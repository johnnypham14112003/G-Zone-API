using System.ComponentModel.DataAnnotations;

namespace GZone.Repository.Models
{
    public class OrderDetail
    {
        [Key]
        public Guid OrderDetailId { get; set; } = Guid.NewGuid();
        public string ProductName { get; set; }
        public string VariantInfo { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsCustomDesign { get; set; }
        public string CustomDesignNote { get; set; }
        public string CustomDesignImage { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public int WarrantyPeriodMonths { get; set; }

        // Foreign keys
        public Guid OrderId { get; set; }

        // Linh hoạt 1 trong 2
        public Guid? ProductVariantId { get; set; }
        public Guid? CustomizationId { get; set; }

        // Navigation properties
        public virtual Order Order { get; set; } = null!;
        public virtual ProductVariant? ProductVariant { get; set; }
        public virtual Customization? Customization { get; set; }
        public virtual ICollection<RatingComment>? RatingComments { get; set; }
        public virtual ICollection<WarrantyClaim>? WarrantyClaims { get; set; }
    }
}
