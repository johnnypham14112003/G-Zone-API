namespace GZone.Service.BusinessModels.Request.Account
{
    public class ChangePasswordRequest
    {
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
