using System.ComponentModel.DataAnnotations;

namespace GZone.Repository.Models
{
    public class UserAddress
    {
        [Key]
        public Guid AddressId { get; set; }

        public string AddressLabel { get; set; } // VD: Nhà riêng, Công ty, Nhà bố mẹ
        public string ReceiverName { get; set; } // Tên người nhận tại địa chỉ này
        public string ReceiverPhone { get; set; } // SĐT người nhận tại địa chỉ này
        public string Address { get; set; } // Số nhà, đường
        public string City { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        
        public bool IsDefault { get; set; } // Địa chỉ mặc định
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Foreign keys
        public Guid AccountId { get; set; } // Khóa ngoại 1-N

        // Navigation properties
        public virtual Account Account { get; set; } = null!;
    }
}
