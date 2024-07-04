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
        private IQueryable<Asset> ApplyFilter(Guid locationId, AssetFilter filter)
        {
            IQueryable<Asset> query = _context.Assets
                                    .Include(x => x.Location)
                                    .Include(x => x.Category)
                                    .Include(x => x.Assignments.OrderBy(a => a.CreatedAt).Take(3))
                                    .ThenInclude(x => x.Assignee)
                                    .Include(x => x.Assignments.OrderBy(a => a.CreatedAt).Take(3))
                                    .ThenInclude(x => x.Assigner);
            if (string.IsNullOrEmpty(filter.search) && !filter.state.HasValue && !filter.category.HasValue)
            {
                query = query.Where(x =>
                (x.State == TypeAssetState.Available || x.State == TypeAssetState.NotAvailable || x.State == TypeAssetState.Assigned)
             && x.LocationId == locationId
             && !x.IsDeleted);
            }
            else
            {
                query = query.Where(x =>
                ((!filter.state.HasValue) || (filter.state.HasValue) && (x.State == filter.state.Value))
             && ((!filter.category.HasValue) || (filter.category.HasValue && x.CategoryId == filter.category))
             && ((string.IsNullOrEmpty(filter.search)) || (!string.IsNullOrEmpty(filter.search) && ((x.AssetCode.ToLower().Contains(filter.search.ToLower()) || x.AssetName.ToLower().Contains(filter.search.ToLower())))))
             && x.LocationId == locationId
             && !x.IsDeleted);
            }
            return query;
        }
        public async Task<IEnumerable<Asset>> GetAllAsync(Func<Asset, object> sortCondition, Guid locationId, AssetFilter filter, int? index, int? size)
        {
            IQueryable<Asset> query = ApplyFilter(locationId, filter);
            IEnumerable<Asset> assets = await query.AsNoTracking().ToListAsync();
            if (filter.order == TypeOrder.Ascending)
            {
                assets = assets.OrderBy(sortCondition);
            }
            else
            {
                assets = assets.OrderByDescending(sortCondition);
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
        public async Task<int> GetTotalCountAsync(Guid locationId, AssetFilter filter)
        {
            return await ApplyFilter(locationId, filter).CountAsync();
        }
        public string CreateAssetCode(string prefix, Guid categoryId)
        {
            var prefixLength = prefix.Length;
            var assetCodes = _context.Assets
                .Where(x => x.CategoryId == categoryId)
                .Select(x => int.Parse(x.AssetCode.Substring(prefixLength)))
                .AsEnumerable();
            var maxAssetCode = _context.Assets.Where(x => x.CategoryId == categoryId).Any() ? assetCodes.Max() : 0;
            int newNumericPart = maxAssetCode + 1;
            return $"{prefix}{newNumericPart:D6}";
        }
    }
}
