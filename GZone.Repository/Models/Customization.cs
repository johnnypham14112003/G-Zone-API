using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GZone.Repository.Models
{
    public class Customization
    {
        [Key]
        public Guid CustomId { get; set; } = Guid.NewGuid();

        public string Name { get; set; }
        public string Sku { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public decimal Weight { get; set; }
        public decimal QuotedPrice { get; set; }
        public string StaffNote { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign keys
        public Guid CustomerId { get; set; }
        public Guid? ConfirmedByStaffId { get; set; }
        public Guid ProductId { get; set; }

        // Navigation properties
        public virtual Account Customer { get; set; } = null!;
        public virtual Account? Staff { get; set; }
        public virtual Product Product { get; set; } = null!;
        public virtual OrderDetail? OrderDetail { get; set; }
        public virtual ICollection<Image>? Images { get; set; }
        public virtual ICollection<Notification>? Notifications { get; set; }
    }
}
