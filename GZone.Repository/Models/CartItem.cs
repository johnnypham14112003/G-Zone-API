using System.ComponentModel.DataAnnotations;

namespace GZone.Repository.Models
{
    public class CartItem
    {
        [Key] public Guid CartItemId { get; set; } = Guid.NewGuid();

        public int Quantity { get; set; }
        public string? CustomDesignNote { get; set; }
        public string? CustomDesignImage { get; set; }
        public decimal CustomPrice { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Foreign keys
        public Guid AccountId { get; set; }
        public Guid ProductVariantId { get; set; }

        // Navigation properties
        public virtual Account Account { get; set; } = null!;
        public virtual ProductVariant ProductVariant { get; set; } = null!;
    }
}
