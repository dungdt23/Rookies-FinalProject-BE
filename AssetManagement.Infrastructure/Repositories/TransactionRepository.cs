using AssetManagement.Application.IRepositories;
using AssetManagement.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore.Storage;

namespace AssetManagement.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AssetManagementDBContext _dbContext;
        private IDbContextTransaction _transaction;

        public TransactionRepository(AssetManagementDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
        }
    }
}
