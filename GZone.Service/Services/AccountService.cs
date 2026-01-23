using GZone.Repository.Interfaces;
using GZone.Repository.Models;
using GZone.Service.Extensions.Exceptions;
using GZone.Service.Extensions.Utils;
using GZone.Service.Interfaces;
using System.Diagnostics;

namespace GZone.Service.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Account> LoginWithEmailPasswordAsync(string email, string password)
        {
            try
            {
                if (BoolUtils.IsEmptyString(email, password))
                    throw new BadRequestException("Email and password are required.");

                //var account = await _unitOfWork.GetAccountRepository()
                //    .GetOneAsync(a => a.Email == email, hasTrackings: false);

                //if (account == null)
                //    throw new UnauthorizedAccessException("Invalid email or password.");

                //if (account.Status.ToLower() == "deleted")
                //    throw new UnauthorizedException("This account has been deleted. Please contact support for assistance.");

                //string hashedInput = HashStringSHA256(password);
                //if (!string.Equals(account.Password, hashedInput, StringComparison.OrdinalIgnoreCase))
                //    throw new UnauthorizedAccessException("Invalid email or password.");

                //return account;
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in method LoginWithEmailPasswordAsync in AccountService: {ex.Message}");
                throw;
            }
        }
    }
}
