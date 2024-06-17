using AssetManagement.Application.Filters;
using AssetManagement.Domain.Entities;

namespace AssetManagement.Application.IRepositories
{
    public interface IAssetRepository : IGenericRepository<Asset>
    {
        Task<IEnumerable<Asset>> GetAllAsync(Func<Asset, object> sortCondition, AssetFilter filter, int? index, int? size);
        Task<int> GetTotalCountAsync(AssetFilter filter);
        Task<string> CreateAssetCode(string prefix);
    }
}
