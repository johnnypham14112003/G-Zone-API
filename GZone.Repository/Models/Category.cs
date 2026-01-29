using System.ComponentModel.DataAnnotations;

namespace GZone.Repository.Models
{
    public class Category
    {
        [Key]
        public Guid CategoryId { get; set; } = Guid.NewGuid();
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public string ImageUrl { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        // Foreign keys
        public Guid? ParentCategoryId { get; set; }

        // Navigation properties
        public virtual Category? ParentCategory { get; set; }
        public virtual ICollection<Category>? SubCategories { get; set; }
        public virtual ICollection<Product>? Products { get; set; }
    }
}
