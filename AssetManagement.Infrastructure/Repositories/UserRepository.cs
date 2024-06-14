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
    public async Task<IEnumerable<User>> GetAllAsync(Func<User,object> condition, UserFilter filter, int? index, int? size)
    {
        IQueryable<User> query = _context.Users
            .Include(x => x.Type)
            .Include(x => x.Location);
        if (index.HasValue && size.HasValue)
        {
            query = query.Skip((index.Value - 1) * size.Value).Take(size.Value);
        }

        var searchString = filter.SearchString;

        query = query.Where(x =>
        (string.IsNullOrEmpty(searchString) || (!string.IsNullOrEmpty(searchString)
        && (x.UserName.Contains(searchString) || x.FirstName.Contains(searchString) || x.StaffCode.Contains(searchString)))));
        var x = _context.Users;

        var users = await query.ToListAsync();


        // check if ascending or descending
        if (filter.IsAscending)
        {
            // except case staff code descending, the rest of cases will ascending default
            return users.OrderBy(condition).ThenBy(x => x.StaffCode);  
        }
        else
        {
            return users.OrderByDescending(condition).ThenBy(x => x.StaffCode);
        }
    }
}
