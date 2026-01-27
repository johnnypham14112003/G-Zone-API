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
        // Other Repository...

        //======================================================================================
        //Constructor
        public UnitOfWork(GZoneDbContext context)
        {
            _context = context;

            //Lazy (initial when needed)
            _accountRepository = new Lazy<IAccountRepository>
                (() => new AccountRepository(context));

            // Other Repository...
        }

        //======================================================================================
        //Methods Expose Repository
        public IAccountRepository GetAccountRepository() => _accountRepository.Value;
        // Other Repository...

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
