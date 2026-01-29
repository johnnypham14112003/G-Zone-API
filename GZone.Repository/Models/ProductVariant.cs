using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GZone.Repository.Models
{
    public class ProductVariant
    {
        [Key]
        public Guid VariantId { get; set; }
        public string Sku { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public decimal AdditionalPrice { get; set; }
        public int StockQuantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int SoldQuantity { get; set; }
        public decimal Weight { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        // Foreign keys
        public Guid ProductId { get; set; }

        // Navigation properties
        public virtual Product Product { get; set; } = null!;
        public virtual CartItem? CartItem { get; set; }
        public virtual OrderDetail? OrderDetails { get; set; }
        public virtual ICollection<Image>? Images { get; set; }
    }
}
