namespace GZone.Service.BusinessModels.Request
{
    public class AuthTokenRequest
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
