using AssetManagement.Application.Filters;
using AssetManagement.Application.IRepositories;
using AssetManagement.Domain.Constants;
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
                                        .Select(asset => new Asset
                                        {
                                            Id = asset.Id,
                                            AssetCode = asset.AssetCode,
                                            AssetName = asset.AssetName,
                                            State = asset.State,
                                            LocationId = asset.LocationId,
                                            CategoryId = asset.CategoryId,
                                            IsDeleted = asset.IsDeleted,
                                            Location = asset.Location,
                                            Category = asset.Category,
                                            Assignments = asset.Assignments.Take(HistoryAssignmentConstant.DefaultDisplay).Select(a => new Assignment
                                            {
                                                Id = a.Id,
                                                Assignee = new User { UserName = a.Assignee.UserName },
                                                Assigner = new User { UserName = a.Assigner.UserName },
                                                AssignedDate = a.AssignedDate,
                                            }).ToList()
                                        });

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
