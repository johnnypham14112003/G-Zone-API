using GZone.Repository.Interfaces;
using System.Data;

namespace GZone.Repository.Base
{
    public interface IUnitOfWork : IDisposable
    {
        // 1. Methods Expose Repository
        IAccountRepository GetAccountRepository();
        IUserAddressRepository GetUserAddressRepository();
        IImageRepository GetImageRepository();
        IProductRepository GetProductRepository();
        ICustomizationRepository GetCustomizationRepository();
        ICategoryRepository GetCategoryRepository();
        IWarrantyClaimRepository GetWarrantyClaimRepository();
        INotificationRepository GetNotificationRepository();
        // Thêm các repo khác:
        IVoucherRepository GetVoucherRepository();
        IUserVoucherRepository GetUserVoucherRepository();
        IOrderVoucherRepository GetOrderVoucherRepository();
        ICustomizationRepository GetCustomizationRepository();
>>>>>>>>> Temporary merge branch 2
        ICustomizationRepository GetCustomizationRepository();
>>>>>>>>> Temporary merge branch 2
        // Thêm các repo khác: IProductRepository GetProductRepository();
        IOrderRepository GetOrderRepository();
        IOrderDetailRepository GetOrderDetailRepository();
        IPaymentTransactionRepository GetPaymentTransactionRepository();

        // 2. Methods Save Changes
        int Complete();
        Task<int> CompleteAsync();

        // 3. Methods Transaction Management
        Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        Task CommitAsync();
        Task RollbackAsync();
    }
}
