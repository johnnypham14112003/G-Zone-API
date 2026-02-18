using System.ComponentModel.DataAnnotations;

namespace GZone.Repository.Models
{
    public class Account
    {
            [Key] public Guid Id { get; set; } = Guid.NewGuid();

            // --- AUTHENTICATION & COMMON INFO ---
            public string? RefreshToken { get; set; }
            public DateTime? RefreshTokenExpiryTime { get; set; }
            public string? AvatarUrl { get; set; }
            public required string Username { get; set; }
            public required string PasswordHash { get; set; }
            public required string Email { get; set; }
            [MaxLength(12)] public string? Phone { get; set; } // Số điện thoại chính chủ
            public string? FullName { get; set; }
            [MaxLength(30)] public required string Role { get; set; }
            [MaxLength(30)] public required string Status { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.Now;

            // --- CUSTOMER SPECIFIC FIELDS (Nullable) ---
            public DateTime? DateOfBirth { get; set; }
            [MaxLength(30)] public string? Gender { get; set; }
            public int? LoyaltyPoints { get; set; } // Null nếu là Staff/Admin

            // --- STAFF SPECIFIC FIELDS (Nullable) ---
            public decimal? Salary { get; set; }
            public DateTime? HireDate { get; set; }

        // Navigation properties
        public virtual ICollection<CartItem>? CartItems { get; set; }
        public virtual ICollection<Customization>? Customizations { get; set; }
        public virtual ICollection<Customization>? ConfirmedCustomizations { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }
        public virtual ICollection<Order>? ManagedOrders { get; set; }
        public virtual ICollection<PaymentTransaction>? Transactions { get; set; }
        public virtual ICollection<RatingComment>? RatingComments { get; set; }
        public virtual ICollection<UserAddress>? UserAddresses { get; set; }
        public virtual ICollection<UserNotification>? UserNotifications { get; set; }
        public virtual ICollection<UserVoucher>? UserVouchers { get; set; }
        public virtual ICollection<Voucher>? CreatedVouchers { get; set; }
        public virtual ICollection<WarrantyClaim>? CreatedClaims { get; set; }
        public virtual ICollection<WarrantyClaim>? ProcessedClaims { get; set; }
    }
}
