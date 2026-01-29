using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;

namespace GZone.Repository.Models
{
    public class Product
    {
        [Key]
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string Sku { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public string Brand { get; set; }
        public string Material { get; set; }
        public string Specifications { get; set; }
        public decimal Weight { get; set; }
        public string Dimension { get; set; }
        public int ViewCount { get; set; }
        public int SoldCount { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public int WarrantyPeriodMonths { get; set; }

        // Foreign keys
        public Guid CategoryId { get; set; }

        // Navigation properties
        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<Customization>? Customizations { get; set; }
        public virtual ICollection<Image>? Images { get; set; }
        public virtual ICollection<ProductVariant>? ProductVariants { get; set; }
        public virtual ICollection<RatingComment>? RatingComments { get; set; }
    }
}
