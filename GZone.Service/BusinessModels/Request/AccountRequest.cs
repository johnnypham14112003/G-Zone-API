using System.ComponentModel.DataAnnotations;

namespace GZone.Service.BusinessModels.Request
{
    public class AccountRequest
    {
        public Guid? Id { get; set; }

        // --- AUTHENTICATION & COMMON INFO ---
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Username { get; set; }
        public string? PasswordHash { get; set; }
        public string? Email { get; set; }
        [MaxLength(12)] public string? Phone { get; set; } // Số điện thoại chính chủ
        public string? FullName { get; set; }
        [MaxLength(30)] public string? Role { get; set; }
        [MaxLength(30)] public string? Status { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // --- CUSTOMER SPECIFIC FIELDS (Nullable) ---
        public DateTime? DateOfBirth { get; set; }
        [MaxLength(30)] public string? Gender { get; set; }
        //public int? LoyaltyPoints { get; set; } // Null nếu là Staff/Admin

        // --- STAFF SPECIFIC FIELDS (Nullable) ---
        //public decimal? Salary { get; set; }s
        //public DateTime? HireDate { get; set; }
    }
}
