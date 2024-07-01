namespace AssetManagement.Application.IRepositories
{
    public interface ITransactionRepository : IDisposable
    {
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }

}
