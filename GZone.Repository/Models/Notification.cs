using System.ComponentModel.DataAnnotations;

namespace GZone.Repository.Models
{
    public class Notification
    {
        [Key]
        public Guid NotificationId { get; set; } = Guid.NewGuid();

        public string NotificationType { get; set; } // System, Order, Promotion...
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign keys
        public Guid ReferenceId { get; set; } // ID của đơn hàng, sản phẩm liên quan...

        // Navigation properties
        public virtual Order? Order { get; set; }
        public virtual Customization? Customization { get; set; }
        public virtual WarrantyClaim? WarrantyClaim { get; set; }
        public virtual ICollection<UserNotification>? UserNotifications { get; set; }
    }
}
