using AssetManagement.Application.Filters;
using AssetManagement.Domain.Entities;

namespace AssetManagement.Application.IRepositories
{
    public interface IAssetRepository : IGenericRepository<Asset>
    {
        Task<IEnumerable<Asset>> GetAllAsync(Func<Asset, object> sortCondition, Guid locationId, AssetFilter filter, int? index, int? size);
        Task<int> GetTotalCountAsync(Guid locationId,AssetFilter filter);
        string CreateAssetCode(string prefix, Guid categoryId);
    }
}
