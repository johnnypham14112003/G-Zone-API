using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request.Account;
using GZone.Service.BusinessModels.Request.Auth;
using GZone.Service.BusinessModels.Response;
using GZone.Service.BusinessModels.Response.Account;
using Microsoft.AspNetCore.Http;

namespace GZone.Service.Interfaces
{
    public interface IAccountService
    {
        Task<ApiResponse<AuthResponse>> LoginByPasswordAsync(AuthRequest authRequest);
        Task<ApiResponse<AuthResponse>> RefreshTokenAsync(AuthTokenRequest request);
        Task RevokeRefreshTokenAsync(Guid accountId);

        //=======================================================================
        Task<ApiResponse<AccountResponse>> GetAccountProfileAsync(Guid accountId);
        Task<ApiResponse<PagedResponse<AccountResponse>>> GetAccountsListAsync(int pageIndex, int pageSize, AccountQuery? query);
        Task<ApiResponse<AccountResponse>> CreateAccountAsync(RegisterRequest request);
        Task<ApiResponse<string>> UpdateAvatarAsync(Guid userId, IFormFile file);
        Task<ApiResponse<bool>> UpdateAccountAsync(AccountRequest request);
        Task<ApiResponse<bool>> DeleteAccountAsync(Guid accountId);
    }
}
