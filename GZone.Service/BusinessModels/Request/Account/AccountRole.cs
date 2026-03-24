using System.ComponentModel.DataAnnotations;

namespace GZone.Service.BusinessModels.Request.Account
{
    public class AccountRole
    {
        public Guid Id { get; set; }
        [MaxLength(30)] public required string Role { get; set; } = "Customer";
    }
}
