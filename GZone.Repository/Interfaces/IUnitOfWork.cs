using System.Data;

namespace GZone.Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // 1. Methods Expose Repository
        IAccountRepository GetAccountRepository();
        // Thêm các repo khác: IProductRepository GetProductRepository();

        // 2. Methods Save Changes
        int Complete();
        Task<int> CompleteAsync();

        // 3. Methods Transaction Management
        Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        Task CommitAsync();
        Task RollbackAsync();
    }
}
