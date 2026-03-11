using GZone.Repository.Interfaces;
using GZone.Repository.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace GZone.Repository.Base
{
    public class UnitOfWork : IUnitOfWork
    {
        //Declare DI
        private readonly GZoneDbContext _context;
        private IDbContextTransaction _transaction;

        private readonly Lazy<IAccountRepository> _accountRepository;
        private readonly Lazy<IUserAddressRepository> _userAddressRepository;
        private readonly Lazy<IImageRepository> _imageRepository;
        private readonly Lazy<IProductRepository> _productRepository;
        private readonly Lazy<ICustomizationRepository> _customizationRepository;
        private readonly Lazy<ICategoryRepository> _categoryRepository;
        private readonly Lazy<IWarrantyClaimRepository> _warrantyClaimRepository;
        private readonly Lazy<INotificationRepository> _notificationRepository;
        private readonly Lazy<IVoucherRepository> _voucherRepository;
        private readonly Lazy<IUserVoucherRepository> _userVoucherRepository;
        private readonly Lazy<IOrderVoucherRepository> _orderVoucherRepository;
        // Other Repository...
        private readonly Lazy<IOrderRepository> _orderRepository;
        private readonly Lazy<IOrderDetailRepository> _orderDetailRepository;
        private readonly Lazy<IPaymentTransactionRepository> _paymentTransactionRepository;

        //======================================================================================
        //Constructor
        public UnitOfWork(GZoneDbContext context)
        {
            _context = context;

            //Lazy (initial when needed)
            _accountRepository = new Lazy<IAccountRepository>
                (() => new AccountRepository(context));
            _userAddressRepository = new Lazy<IUserAddressRepository>
                (() => new UserAddressRepository(context));
            _imageRepository = new Lazy<IImageRepository>
                (() => new ImageRepository(context));
            _productRepository = new Lazy<IProductRepository>
                (() => new ProductRepository(context));
            _customizationRepository = new Lazy<ICustomizationRepository>
                (() => new CustomizationRepository(context));
            _categoryRepository = new Lazy<ICategoryRepository>
                (() => new CategoryRepository(context));
            _warrantyClaimRepository = new Lazy<IWarrantyClaimRepository>
                (() => new WarrantyClaimRepository(context));
            _notificationRepository = new Lazy<INotificationRepository>
                (() => new NotificationRepository(context));
            _voucherRepository = new Lazy<IVoucherRepository>
                (() => new VoucherRepository(context));
            _userVoucherRepository = new Lazy<IUserVoucherRepository>
                (() => new UserVoucherRepository(context));
            _orderVoucherRepository = new Lazy<IOrderVoucherRepository>
                (() => new OrderVoucherRepository(context));

            // Other Repository...
            _orderRepository = new Lazy<IOrderRepository>
                (() => new OrderRepository(context));
            _orderDetailRepository = new Lazy<IOrderDetailRepository>
                (() => new OrderDetailRepository(context));
            _paymentTransactionRepository = new Lazy<IPaymentTransactionRepository>
                (() => new PaymentTransactionRepository(context));
        }

        //======================================================================================
        //Methods Expose Repository
        public IAccountRepository GetAccountRepository() => _accountRepository.Value;
        public IUserAddressRepository GetUserAddressRepository() => _userAddressRepository.Value;
        public IImageRepository GetImageRepository() => _imageRepository.Value;
        public IProductRepository GetProductRepository() => _productRepository.Value;
        public ICustomizationRepository GetCustomizationRepository() => _customizationRepository.Value;
        public ICategoryRepository GetCategoryRepository() => _categoryRepository.Value;
        public IWarrantyClaimRepository GetWarrantyClaimRepository() => _warrantyClaimRepository.Value;
        public INotificationRepository GetNotificationRepository() => _notificationRepository.Value;
        public IVoucherRepository GetVoucherRepository() => _voucherRepository.Value;
        public IUserVoucherRepository GetUserVoucherRepository() => _userVoucherRepository.Value;
        public IOrderVoucherRepository GetOrderVoucherRepository() => _orderVoucherRepository.Value;
        // Other Repository...
        public IOrderRepository GetOrderRepository() => _orderRepository.Value;
        public IOrderDetailRepository GetOrderDetailRepository() => _orderDetailRepository.Value;
        public IPaymentTransactionRepository GetPaymentTransactionRepository() => _paymentTransactionRepository.Value;

        //======================================================================================
        //Other Methods
        public int Complete() => _context.SaveChanges();
        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose()
        {
            _context.Dispose();
            // SuppressFinalize để GC không gọi Finalizer, giúp tối ưu hiệu năng
            GC.SuppressFinalize(this);
        }

        public async Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            _transaction = await _context.Database.BeginTransactionAsync(isolationLevel);
        }

        public async Task CommitAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
            }
        }
    }
}
