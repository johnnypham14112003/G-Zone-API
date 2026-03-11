using GZone.Repository.Base;
using GZone.Repository.Models;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request;
using GZone.Service.BusinessModels.Response;
using GZone.Service.Extensions.Exceptions;
using GZone.Service.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace GZone.Service.Services
{
    public class UserVoucherService : IUserVoucherService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserVoucherService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<UserVoucherResponse>>> GetVouchersByAccountAsync(Guid accountId)
        {
            // Ki?m tra account t?n t?i
            var accountExists = await _unitOfWork.GetAccountRepository().AnyAsync(a => a.Id == accountId);
            if (!accountExists)
                throw new NotFoundException("Not found any account match the Id!");

            var userVouchers = await _unitOfWork.GetUserVoucherRepository().GetListAsync(
                uv => uv.AccountId == accountId,
                include: q => q.Include(uv => uv.Voucher));

            if (userVouchers is null || userVouchers.Count == 0)
                return ApiResponse<List<UserVoucherResponse>>.Success([], "No vouchers found for this account.");

            var response = userVouchers.Select(uv => new UserVoucherResponse
            {
                AccountId = uv.AccountId,
                VoucherId = uv.VoucherId,
                IsUsed = uv.IsUsed,
                VoucherCode = uv.Voucher.VoucherCode,
                VoucherName = uv.Voucher.VoucherName,
                DiscountType = uv.Voucher.DiscountType,
                DiscountValue = uv.Voucher.DiscountValue,
                MaxDiscountAmount = uv.Voucher.MaxDiscountAmount,
                MinOrderAmount = uv.Voucher.MinOrderAmount,
                StartDate = uv.Voucher.StartDate,
                EndDate = uv.Voucher.EndDate,
                IsActive = uv.Voucher.IsActive
            }).ToList();

            return ApiResponse<List<UserVoucherResponse>>.Success(response);
        }

        public async Task<ApiResponse<List<UserVoucherResponse>>> GetAccountsByVoucherAsync(Guid voucherId)
        {
            // Ki?m tra voucher t?n t?i
            var voucherExists = await _unitOfWork.GetVoucherRepository().AnyAsync(v => v.VoucherId == voucherId);
            if (!voucherExists)
                throw new NotFoundException("Not found any voucher match the Id!");

            var userVouchers = await _unitOfWork.GetUserVoucherRepository().GetListAsync(
                uv => uv.VoucherId == voucherId,
                include: q => q.Include(uv => uv.Voucher));

            if (userVouchers is null || userVouchers.Count == 0)
                return ApiResponse<List<UserVoucherResponse>>.Success([], "No users found for this voucher.");

            var response = userVouchers.Select(uv => new UserVoucherResponse
            {
                AccountId = uv.AccountId,
                VoucherId = uv.VoucherId,
                IsUsed = uv.IsUsed,
                VoucherCode = uv.Voucher.VoucherCode,
                VoucherName = uv.Voucher.VoucherName,
                DiscountType = uv.Voucher.DiscountType,
                DiscountValue = uv.Voucher.DiscountValue,
                MaxDiscountAmount = uv.Voucher.MaxDiscountAmount,
                MinOrderAmount = uv.Voucher.MinOrderAmount,
                StartDate = uv.Voucher.StartDate,
                EndDate = uv.Voucher.EndDate,
                IsActive = uv.Voucher.IsActive
            }).ToList();

            return ApiResponse<List<UserVoucherResponse>>.Success(response);
        }

        public async Task<ApiResponse<bool>> AssignVoucherToUserAsync(UserVoucherRequest request)
        {
            // 1. Ki?m tra account t?n t?i
            var accountExists = await _unitOfWork.GetAccountRepository().AnyAsync(a => a.Id == request.AccountId);
            if (!accountExists)
                throw new NotFoundException("Not found any account match the Id!");

            // 2. Ki?m tra voucher t?n t?i và c?n hi?u l?c
            var voucher = await _unitOfWork.GetVoucherRepository().GetByIdAsync(request.VoucherId);
            if (voucher is null)
                throw new NotFoundException("Not found any voucher match the Id!");

            if (!voucher.IsActive)
                throw new BadRequestException("Voucher is no longer active!");

            if (voucher.EndDate < DateTime.Now)
                throw new BadRequestException("Voucher has expired!");

            if (voucher.CurrentUsageCount >= voucher.MaxUsageTotal)
                throw new BadRequestException("Voucher has reached its maximum usage limit!");

            // 3. Ki?m tra user ð? có voucher này chýa
            var alreadyAssigned = await _unitOfWork.GetUserVoucherRepository()
                .AnyAsync(uv => uv.AccountId == request.AccountId && uv.VoucherId == request.VoucherId);

            if (alreadyAssigned)
                throw new ConflictException("This voucher has already been assigned to the user!");

            // 4. Gán voucher
            var userVoucher = new UserVoucher
            {
                AccountId = request.AccountId,
                VoucherId = request.VoucherId,
                IsUsed = false
            };

            await _unitOfWork.GetUserVoucherRepository().AddAsync(userVoucher);
            await _unitOfWork.CompleteAsync();

            return ApiResponse<bool>.Success(true, "Voucher assigned to user successfully!");
        }

        public async Task<ApiResponse<bool>> RevokeVoucherFromUserAsync(UserVoucherRequest request)
        {
            // 1. Ki?m tra b?n ghi t?n t?i
            var userVoucher = await _unitOfWork.GetUserVoucherRepository()
                .GetOneAsync(uv => uv.AccountId == request.AccountId && uv.VoucherId == request.VoucherId);

            if (userVoucher is null)
                throw new NotFoundException("This voucher is not assigned to the specified user!");

            // 2. Không thu h?i n?u ð? s? d?ng
            if (userVoucher.IsUsed)
                throw new BadRequestException("Cannot revoke a voucher that has already been used!");

            // 3. Xóa
            await _unitOfWork.GetUserVoucherRepository().DeleteAsync(userVoucher);
            await _unitOfWork.CompleteAsync();

            return ApiResponse<bool>.Success(true, "Voucher revoked from user successfully!");
        }
    }
}
