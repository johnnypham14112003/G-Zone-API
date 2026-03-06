using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request.UserAddress;
using GZone.Service.BusinessModels.Response.UserAddress;

namespace GZone.Service.Interfaces
{
    public interface IUserAddressService
    {
        public Task<ApiResponse<UserAddressResponse>> GetUserAddressByIdAsync(Guid addressId);
        public Task<ApiResponse<List<UserAddressResponse>>> GetUserAddressesListAsync(Guid accountId);
        public Task<ApiResponse<UserAddressResponse>> CreateUserAddressAsync(UserAddressRequest request);
        public Task<ApiResponse<bool>> UpdateUserAddressAsync(UserAddressRequest request);
        public Task<ApiResponse<bool>> DeleteUserAddressAsync(Guid addressId);
    }
}
