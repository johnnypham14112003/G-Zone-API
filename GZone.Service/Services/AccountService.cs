using GZone.Repository.Base;
using GZone.Repository.Models;
using GZone.Service.BusinessModels.Generic;
using GZone.Service.BusinessModels.Request;
using GZone.Service.BusinessModels.Response;
using GZone.Service.Extensions.Exceptions;
using GZone.Service.Extensions.Utils;
using GZone.Service.Interfaces;
using Mapster;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using System.Security.Claims;

namespace GZone.Service.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;

        public AccountService(IUnitOfWork unitOfWork, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        public async Task<ApiResponse<AuthResponse>> LoginByPasswordAsync(AuthRequest authRequest)
        {
            if (BoolUtils.IsValidEmail(authRequest.Email) == false)
                throw new BadRequestException("Invalid Email Format!");

            // Check account in database
            var existAccount = await _unitOfWork.GetAccountRepository().GetOneAsync(
                acc => acc.Email.ToLower().Equals(authRequest.Email.ToLower()));

            if (existAccount is null)
                throw new NotFoundException("Not Found Account match email!");

            //Hash Password
            var securedPassword = StringUtils.HashStringSHA256(authRequest.Password);

            // If password not match
            if (!securedPassword.Equals(existAccount.PasswordHash))
                throw new BadRequestException("Wrong Password!");

            //Generate Tokens
            var accessToken = _tokenService.GenerateAccessToken(existAccount);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var expireTimes = _tokenService.GetExpirationTimes();

            // Lưu refresh token vào database
            existAccount.RefreshToken = refreshToken;
            existAccount.RefreshTokenExpiryTime = DateTime.Now.AddDays(expireTimes.refreshDay);

            await _unitOfWork.CompleteAsync();
            var authResponse = new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = expireTimes.accessMinute * 60,//convert to seconds
                Email = existAccount.Email,
                UserName = existAccount.Username,
                Role = existAccount.Role,
                Avatar = existAccount.AvatarUrl
            };
            return ApiResponse<AuthResponse>.Success(authResponse);
        }

        public async Task<ApiResponse<AuthResponse>> RefreshTokenAsync(AuthTokenRequest request)
        {
            //Decode token to get accountId
            var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
            var accountId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get accountId from token
            if (accountId == null || accountId.Equals(Guid.Empty.ToString()))
                throw new UnauthorizedException("Invalid Account ID");

            // Get account from accountId
            var existAccount = await _unitOfWork.GetAccountRepository().GetByIdAsync(Guid.Parse(accountId));
            if (existAccount == null ||
                existAccount.RefreshToken != request.RefreshToken ||
                existAccount.RefreshTokenExpiryTime <= DateTime.Now)
            {
                throw new SecurityTokenException("Invalid refresh token");
            }

            var newAccessToken = _tokenService.GenerateAccessToken(existAccount);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var expireTimes = _tokenService.GetExpirationTimes();

            existAccount.RefreshToken = newRefreshToken;
            existAccount.RefreshTokenExpiryTime = DateTime.Now.AddDays(expireTimes.refreshDay);

            await _unitOfWork.CompleteAsync();

            var authResponse = new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresIn = expireTimes.accessMinute * 60,//convert to seconds
                Email = existAccount.Email,
                UserName = existAccount.Username,
                Role = existAccount.Role,
            };
            return ApiResponse<AuthResponse>.Success(authResponse);
        }

        public async Task RevokeRefreshTokenAsync(Guid accountId)
        {
            var account = await _unitOfWork.GetAccountRepository().GetByIdAsync(accountId);
            if (account != null)
            {
                account.RefreshToken = null;
                account.RefreshTokenExpiryTime = null;
                await _unitOfWork.CompleteAsync();
            }
        }

        //=================================================================================================
        public async Task<ApiResponse<Account>> GetAccountProfileAsync(Guid accountId)
        {
            var account = await _unitOfWork.GetAccountRepository().GetByIdAsync(accountId);

            if (account == null)
                throw new NotFoundException("Not found any account match the Id!");

            return ApiResponse<Account>.Success(account);
        }

        /*
        public async Task<ApiResponse<PagedResponse<AccountResponse>>> GetAccountsListAsync(int pageIndex, int pageSize, modelClass? query)
        {
            if (pageIndex <= 0) pageIndex = 1;
            if (pageSize <= 0) pageSize = 10;

            Expression<Func<Account, bool>>? predicate = null;

            if (!string.IsNullOrWhiteSpace(query.column))
            {
                predicate = q => q.Code.ToLower().Contains(query.column.ToLower()) ||
                                q.Email.ToLower().Contains(query.column.ToLower()) ||
                                q.Name.ToLower().Contains(query.column.ToLower()) ||
                                q.PhoneNumber!.Contains(query.column);
            }
        // Nếu muốn thêm filter theo Role, Status,... thì có thể mở rộng thêm vào predicate ở đây
        if (!string.IsNullOrEmpty(query.column))
        {
        --- And này là hàm ở trong ExpressionExtensions.cs (Services/Extensions/ExpressionExtensions.cs) ---
            predicate = predicate.And(c => c.Type.Contains(query.column));
        }

        if (!string.IsNullOrEmpty(query.column))
        {
            predicate = predicate.And(c => c.Name.Contains(query.column));
        }
        //==============================================================================================

            Func<IQueryable<Account>, IOrderedQueryable<Account>> orderBy =
                q => q.OrderByDescending(x => x.CreatedAt);

            var accounts = await _unitOfWork.AccountRepository.GetPagedAsync(
                pageIndex, pageSize, predicate, orderBy);

            var totalCount = await _unitOfWork.AccountRepository.CountAsync();

            var response = accounts.Adapt<List<AccountResponse>>();
            var pagedResponse = new PagedResponse<AccountResponse>
            {
                DataList = response,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            return ApiResponse<PagedResponse<AccountResponse>>.Success(account);
        }*/

        public async Task<ApiResponse<Account>> CreateAccountAsync(RegisterRequest request)
        {
            // 1. Validate Email format
            if (!BoolUtils.IsValidEmail(request.Email))
            {
                throw new BadRequestException("Invalid email!");
            }

            // 2. Check Duplicate Email
            var isEmailExist = await _unitOfWork.GetAccountRepository().AnyAsync(x => x.Email == request.Email);
            if (isEmailExist)
            {
                // 409 Conflict: Tài nguyên đã tồn tại
                throw new ConflictException("Email already signed-up.");
            }

            // 3. Check Duplicate Username (Optional)
            var isUserExist = await _unitOfWork.GetAccountRepository().AnyAsync(x => x.Username == request.UserName);
            if (isUserExist)
            {
                throw new ConflictException("This Username already existed!");
            }

            // 4. Create Entity
            var newAccount = new Account
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Username = request.UserName, // Giả sử model DB là Name
                PasswordHash = StringUtils.HashStringSHA256(request.Password),
                AvatarUrl = request.Avatar,
                CreatedAt = DateTime.Now,
                IsActive = true, // Mặc định kích hoạt
                Role = "Customer",
                Status = "Active"
            };

            // 5. Save to DB
            await _unitOfWork.GetAccountRepository().AddAsync(newAccount);
            await _unitOfWork.CompleteAsync();

            return ApiResponse<Account>.Success(newAccount, "Create account successfully!");
        }

        public async Task<ApiResponse<bool>> UpdateAccountAsync(AccountRequest request)
        {
            // 1. Check exist Account
            var account = await _unitOfWork.GetAccountRepository().GetByIdAsync(request.Id);
            if (account is null)
            {
                throw new NotFoundException("Not found any account match the Id!");
            }

            // 2. Validate Duplicate Email (nếu đổi email)
            // Logic: Nếu email thay đổi VÀ email mới đã thuộc về người khác
            if (!account.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase))
            {
                if (!BoolUtils.IsValidEmail(request.Email))
                    throw new BadRequestException("Invalid email!");

                var isEmailTaken = await _unitOfWork.GetAccountRepository()
                    .AnyAsync(x => x.Email == request.Email && x.Id != request.Id);

                if (isEmailTaken)
                {
                    throw new ConflictException("This Email has been used by other!");
                }
            }

            // 3. Update fields
            var tempPassword = account.PasswordHash;
            request.Adapt(account);

            account.PasswordHash = tempPassword; // Retain existing password

            // 4. Save
            await _unitOfWork.CompleteAsync();

            return ApiResponse<bool>.Success(true, "Update successfully!");
        }

        public async Task<ApiResponse<bool>> DeleteAccountAsync(Guid accountId)
        {
            // 1. Check exist
            var account = await _unitOfWork.GetAccountRepository().GetByIdAsync(accountId);
            if (account is null)
            {
                throw new NotFoundException("Not found any account match the Id!");
            }

            // 2. Perform Delete
            // Cách 1: Hard Delete (Xóa vĩnh viễn khỏi DB)
            // _unitOfWork.AccountRepository.Remove(account);

            // Cách 2: Soft Delete (Đánh dấu đã xóa)
            // Giả sử Entity Account có field IsDeleted
            // account.IsDeleted = true;
            // _unitOfWork.AccountRepository.Update(account);

            await _unitOfWork.GetAccountRepository().DeleteAsync(account);

            await _unitOfWork.CompleteAsync();

            return ApiResponse<bool>.Success(true, "Delete Successfully!");
        }
    }
}
