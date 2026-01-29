using System.ComponentModel.DataAnnotations;

namespace GZone.Repository.Models
{
    public class Image
    {
        [Key]
        public Guid ImageId { get; set; }
        public string ImageUrl { get; set; }
        public string AltText { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign keys
        public Guid? ProductId { get; set; } // Nullable vì ảnh có thể thuộc về Product
        public Guid? CustomId { get; set; } // Nullable vì ảnh có thể thuộc về Customization
        public Guid? ProductVariantId { get; set; } // Nullable vì ảnh có thể thuộc về ProductVariant
        public Guid? RatingId { get; set; } // Nullable vì ảnh có thể thuộc về Rating

        // Navigation properties
        public virtual Product? Product { get; set; }
        public virtual Customization? Customization { get; set; }
        public virtual ProductVariant? ProductVariant { get; set; }
        public virtual RatingComment? Rating { get; set; }
    }
}
