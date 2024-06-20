using AssetManagement.Application.Filters;
using AssetManagement.Application.IRepositories;
using AssetManagement.Domain.Entities;
using AssetManagement.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly AssetManagementDBContext _context;
    private readonly DbSet<User> _dbSet;

    public UserRepository(AssetManagementDBContext context) : base(context)
    {
        _context = context;
        _dbSet = context.Set<User>();
    }

    public string GenerateStaffCode()
    {
        var staffcodes = _dbSet.Select(x => int.Parse(x.StaffCode.Substring(2))).AsEnumerable();
        int maxId = _dbSet.Any() ? staffcodes.Max() : 0;
        return $"SD{(maxId + 1).ToString("D4")}";
    }

    public string GenerateUserName(string fullName)
    {
        var nameParts = fullName.Split(' ');
        var lastName = nameParts.Last();
        var initials = string.Join("", nameParts.Take(nameParts.Length - 1).Select(n => n[0])).ToLower();
        var baseUserName = $"{lastName.ToLower()}{initials}";

        int suffix = 1;
        var userName = baseUserName;
        while (_dbSet.Any(u => u.UserName == userName))
        {
            userName = $"{baseUserName}{suffix}";
            suffix++;
        }

        return userName;
    }
    private IQueryable<User> ApplyFilter(Guid locationId, UserFilter filter)
    {
        IQueryable<User> query = _context.Users.Where(x => !x.IsDeleted && x.LocationId == locationId);
        if (filter.UserType.HasValue)
        {
            var type = Enum.GetName(typeof(UserType), filter.UserType.Value);
            query = query.Where(x => x.Type.TypeName.Equals(type));
        }
        var searchString = filter.SearchString;

        query = query.Where(x =>
        (string.IsNullOrEmpty(searchString) || (!string.IsNullOrEmpty(searchString)
        && (x.UserName.Contains(searchString) || x.FirstName.Contains(searchString)
        || x.LastName.Contains(searchString) || x.StaffCode.Contains(searchString)
        || ((x.LastName + x.FirstName).ToLower().Replace(" ", "").Trim()).Contains(searchString.ToLower().Replace(" ", "").Trim())))));
        return query;
    }
    public async Task<IEnumerable<User>> GetAllAsync(Func<User, object> condition, Guid locationId, UserFilter filter, int? index, int? size)
    {
        IQueryable<User> query = ApplyFilter(locationId, filter);
        IEnumerable<User> users = await query.Include(x => x.Location)
                                            .Include(x => x.Type)
                                             .AsNoTracking()
                                            .ToListAsync();
        if (filter.IsAscending)
        {
            // Sort based on the condition and then by StaffCode in ascending order
            users = users.OrderBy(condition).ThenBy(x => x.StaffCode);
        }
        else
        {
            // Sort based on the condition and then by StaffCode in descending order
            users = users.OrderByDescending(condition).ThenBy(x => x.StaffCode);
        }

        //handle pagination
        if (index.HasValue && size.HasValue)
        {
            users = users.Skip((index.Value - 1) * size.Value).Take(size.Value);
        }

        return users;
    }
    public async Task<int> GetTotalCountAsync(Guid locationId, UserFilter filter)
    {
        return await ApplyFilter(locationId, filter).CountAsync();
    }
}
