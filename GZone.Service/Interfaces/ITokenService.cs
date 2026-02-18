using GZone.Repository.Models;
using System.Security.Claims;

namespace GZone.Service.Interfaces
{
    public interface ITokenService
    {
        (int accessMinute, int refreshDay) GetExpirationTimes();
        string GenerateAccessToken(Account account);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
