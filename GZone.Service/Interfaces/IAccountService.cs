using GZone.Repository.Models;

namespace GZone.Service.Interfaces
{
    public interface IAccountService
    {
        Task<Account> LoginWithEmailPasswordAsync(string email, string password);
    }
}
