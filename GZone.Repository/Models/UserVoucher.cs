namespace GZone.Repository.Models
{
    public class UserVoucher
    {
        public bool IsUsed { get; set; }

        // Foreign keys
        public Guid AccountId { get; set; }
        public Guid VoucherId { get; set; }

        // Navigation Properties
        public virtual Account Account { get; set; } = null!;
        public virtual Voucher Voucher { get; set; } = null!;
    }
}
