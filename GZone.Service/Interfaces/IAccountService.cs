using GZone.Repository.Models;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request;
using GZone.Service.BusinessModels.Request.Account;
using GZone.Service.BusinessModels.Response;

namespace GZone.Service.Interfaces
{
    public interface IAccountService
    {
        Task<ApiResponse<AuthResponse>> LoginByPasswordAsync(AuthRequest authRequest);
        Task<ApiResponse<AuthResponse>> RefreshTokenAsync(AuthTokenRequest request);
        Task RevokeRefreshTokenAsync(Guid accountId);

        //=======================================================================
        Task<ApiResponse<Account>> GetAccountProfileAsync(Guid accountId);
        Task<ApiResponse<PagedResponse<AccountResponse>>> GetAccountsListAsync(int pageIndex, int pageSize, AccountQuery? query);
        Task<ApiResponse<Account>> CreateAccountAsync(RegisterRequest request);
        Task<ApiResponse<bool>> UpdateAccountAsync(AccountRequest request);
        Task<ApiResponse<bool>> DeleteAccountAsync(Guid accountId);
    }
}
