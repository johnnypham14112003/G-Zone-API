using GZone.Repository.Models;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request;
using GZone.Service.BusinessModels.Response;

namespace GZone.Service.Interfaces
{
    public interface IUserVoucherService
    {
        // L?y danh sách voucher c?a 1 user
        Task<ApiResponse<List<UserVoucherResponse>>> GetVouchersByAccountAsync(Guid accountId);

        // L?y danh sách user s? h?u 1 voucher (Admin/Staff)
        Task<ApiResponse<List<UserVoucherResponse>>> GetAccountsByVoucherAsync(Guid voucherId);

        // T?ng / phân ph?i voucher cho user
        Task<ApiResponse<bool>> AssignVoucherToUserAsync(UserVoucherRequest request);

        // Thu h?i voucher kh?i user
        Task<ApiResponse<bool>> RevokeVoucherFromUserAsync(UserVoucherRequest request);
    }
}
