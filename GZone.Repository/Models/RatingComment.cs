using System.ComponentModel.DataAnnotations;

namespace GZone.Repository.Models
{
    public class RatingComment
    {
        [Key]
        public Guid RatingId { get; set; }
        public int RatingScore { get; set; }
        public string Comment { get; set; }
        public string AdminReply { get; set; }
        public DateTime? ReplyAt { get; set; }
        public int HelpfulCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsVerifiedPurchase { get; set; }
        public bool IsHidden { get; set; }

        // Foreign keys
        public Guid ProductId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid OrderDetailId { get; set; }

        // Navigation properties
        public virtual Product Product { get; set; } = null!;
        public virtual Account Customer { get; set; } = null!;
        public virtual OrderDetail OrderDetail { get; set; } = null!;
        public virtual ICollection<Image>? RatingImages { get; set; }
    }
}
