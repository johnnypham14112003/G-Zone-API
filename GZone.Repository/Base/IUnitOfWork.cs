using GZone.Repository.Interfaces;
using System.Data;

namespace GZone.Repository.Base
{
    public interface IUnitOfWork : IDisposable
    {
        // 1. Methods Expose Repository
        IAccountRepository GetAccountRepository();
        IProductRepository GetProductRepository();
        ICategoryRepository GetCategoryRepository();
        IWarrantyClaimRepository GetWarrantyClaimRepository();
        INotificationRepository GetNotificationRepository();
        // Thêm các repo khác:

        // 2. Methods Save Changes
        int Complete();
        Task<int> CompleteAsync();

        // 3. Methods Transaction Management
        Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        Task CommitAsync();
        Task RollbackAsync();
    }
}
