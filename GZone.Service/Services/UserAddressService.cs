using GZone.Repository.Base;
using GZone.Repository.Models;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request.UserAddress;
using GZone.Service.BusinessModels.Response.UserAddress;
using GZone.Service.Extensions.Exceptions;
using GZone.Service.Interfaces;
using Mapster;

namespace GZone.Service.Services
{
    public class UserAddressService : IUserAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserAddressService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<UserAddressResponse>> GetUserAddressByIdAsync(Guid addressId)
        {
            var address = await _unitOfWork.GetUserAddressRepository().GetByIdAsync(addressId);

            if (address == null)
                throw new NotFoundException("Not found any user address match the Id!");

            return ApiResponse<UserAddressResponse>.Success(address.Adapt<UserAddressResponse>());
        }

        public async Task<ApiResponse<List<UserAddressResponse>>> GetUserAddressesListAsync(Guid accountId)
        {
            var addresses = await _unitOfWork.GetUserAddressRepository()
                .GetListAsync(x => x.AccountId == accountId, hasTrackings: false);

            if (addresses == null || !addresses.Any())
                return ApiResponse<List<UserAddressResponse>>.Success(new List<UserAddressResponse>());

            // Sắp xếp: Địa chỉ mặc định lên đầu, sau đó theo ngày tạo mới nhất
            var sortedAddresses = addresses.OrderByDescending(x => x.IsDefault)
                                           .ThenByDescending(x => x.CreatedAt)
                                           .ToList();

            var response = sortedAddresses.Adapt<List<UserAddressResponse>>();

            return ApiResponse<List<UserAddressResponse>>.Success(response);
        }

        public async Task<ApiResponse<UserAddressResponse>> CreateUserAddressAsync(UserAddressRequest request)
        {
            if (request.IsDefault)
            {
                // Sử dụng GetListAsync với hasTrackings = true để Entity Framework theo dõi sự thay đổi
                var existingDefaults = await _unitOfWork.GetUserAddressRepository()
                    .GetListAsync(x => x.AccountId == request.AccountId && x.IsDefault, hasTrackings: true);

                if (existingDefaults != null)
                {
                    foreach (var addr in existingDefaults)
                    {
                        addr.IsDefault = false;
                    }
                }
            }

            var newAddress = new UserAddress
            {
                AddressId = Guid.NewGuid(),
                AccountId = request.AccountId,
                AddressLabel = request.AddressLabel,
                ReceiverName = request.ReceiverName,
                ReceiverPhone = request.ReceiverPhone,
                Address = request.Address,
                City = request.City,
                District = request.District,
                Ward = request.Ward,
                IsDefault = request.IsDefault,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.GetUserAddressRepository().AddAsync(newAddress);
            await _unitOfWork.CompleteAsync();

            return ApiResponse<UserAddressResponse>.Success(newAddress.Adapt<UserAddressResponse>(), "Create user address successfully!");
        }

        public async Task<ApiResponse<bool>> UpdateUserAddressAsync(UserAddressRequest request)
        {
            var address = await _unitOfWork.GetUserAddressRepository().GetByIdAsync(request.AddressId);
            if (address is null)
            {
                throw new NotFoundException("Not found any user address match the Id!");
            }

            if (request.IsDefault && !address.IsDefault)
            {
                var existingDefaults = await _unitOfWork.GetUserAddressRepository()
                    .GetListAsync(x => x.AccountId == address.AccountId && x.IsDefault && x.AddressId != address.AddressId, hasTrackings: true);

                if (existingDefaults != null)
                {
                    foreach (var addr in existingDefaults)
                    {
                        addr.IsDefault = false;
                    }
                }
            }

            request.Adapt(address);
            address.UpdatedAt = DateTime.Now;

            await _unitOfWork.CompleteAsync();

            return ApiResponse<bool>.Success(true, "Update successfully!");
        }

        public async Task<ApiResponse<bool>> DeleteUserAddressAsync(Guid addressId)
        {
            var address = await _unitOfWork.GetUserAddressRepository().GetByIdAsync(addressId);
            if (address is null)
            {
                throw new NotFoundException("Not found any user address match the Id!");
            }

            await _unitOfWork.GetUserAddressRepository().DeleteAsync(address);
            await _unitOfWork.CompleteAsync();

            return ApiResponse<bool>.Success(true, "Delete Successfully!");
        }
    }
}
