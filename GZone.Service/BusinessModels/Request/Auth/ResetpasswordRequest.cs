namespace GZone.Service.BusinessModels.Request.Auth
{
    public class ResetpasswordRequest
    {
        public Guid Id { get; set; }
        public required string NewPassword { get; set; }
    }
}
