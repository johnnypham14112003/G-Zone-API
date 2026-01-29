using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GZone.Repository.Models
{
    public class UserNotification
    {
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign keys
        public Guid AccountId { get; set; }
        public Guid NotificationId { get; set; }

        // Navigation Properties
        public virtual Account Account { get; set; } = null!;
        public virtual Notification Notification { get; set; } = null!;
    }
}
