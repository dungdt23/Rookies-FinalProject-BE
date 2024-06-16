using AssetManagement.Application.Filters;
using AssetManagement.Application.IRepositories;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AssetManagement.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Infrastructure.Repositories
{
    public class AssetRepository : GenericRepository<Asset>, IAssetRepository
    {
        private readonly AssetManagementDBContext _context;
        public AssetRepository(AssetManagementDBContext context) : base(context)
        {
            _context = context;
        }
        private IQueryable<Asset> ApplyFilter( AssetFilter filter)
        {
            IQueryable<Asset> query = _context.Assets
                .Include(x => x.Location)
                .Include(x => x.Category)
                .Where(x =>
                ((!filter.state.HasValue) || (filter.state.HasValue) && (x.State == filter.state.Value))
             && ((string.IsNullOrEmpty(filter.category)) || (!string.IsNullOrEmpty(filter.category) && x.Category.CategoryName.Equals(filter.category)))
             && ((string.IsNullOrEmpty(filter.search)) || (!string.IsNullOrEmpty(filter.search) && (x.AssetCode.Contains(filter.search) || x.AssetName.Contains(filter.search))))
             && x.LocationId == filter.locationId
             && !x.IsDeleted);
            return query;
        }  
        public async Task<IEnumerable<Asset>> GetAllAsync(Func<Asset,object> sortCondition, AssetFilter filter, int? index, int? size)
        {
            IQueryable<Asset> query = ApplyFilter(filter);
            IEnumerable<Asset> assets = await query.ToListAsync();
            if (filter.order == TypeOrder.Ascending)
            {
                assets.OrderBy(sortCondition);
            }
            else
            {
                assets.OrderByDescending(sortCondition);
            }
            if (index.HasValue && size.HasValue)
            {
                return assets.Skip((index.Value - 1) * size.Value).Take(size.Value);
            }
            else
            {
                return assets;
            }
        }
        public async Task<int> GetTotalCountAsync(AssetFilter filter)
        {
            return await ApplyFilter(filter).CountAsync();
        }
    }
}
