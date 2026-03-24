using System.ComponentModel.DataAnnotations;

namespace GZone.Service.BusinessModels.Request.Account
{
    public class AccountRequest
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        [MaxLength(12)] public string? Phone { get; set; } // Số điện thoại chính chủ
        public string? FullName { get; set; }
        [MaxLength(30)] public string? Status { get; set; }
        public bool IsActive { get; set; }

        // --- CUSTOMER SPECIFIC FIELDS (Nullable) ---
        public DateTime? DateOfBirth { get; set; }
        [MaxLength(30)] public string? Gender { get; set; }
    }
}
