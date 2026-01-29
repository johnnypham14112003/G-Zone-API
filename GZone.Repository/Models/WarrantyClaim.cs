using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GZone.Repository.Models
{
    public class WarrantyClaim
    {
        [Key]
        public Guid ClaimId { get; set; }
        public string ClaimNumber { get; set; }
        public string IssueDescription { get; set; }
        public string ClaimStatus { get; set; }
        public DateTime ClaimDate { get; set; }
        public DateTime? ResolutionDate { get; set; }
        public string ResolutionNotes { get; set; }
        public decimal RepairCost { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Status { get; set; }
        
        // Foreign keys
        public Guid CustomerId { get; set; }
        public Guid? ProcessedByStaffId { get; set; }
        public Guid OrderDetailId { get; set; }

        // Navigation properties
        public virtual Account Customer { get; set; } = null!;
        public virtual Account? Staff { get; set; }
        public virtual OrderDetail OrderDetail { get; set; } = null!;
        public virtual ICollection<Notification>? Notifications { get; set; }
    }
}
