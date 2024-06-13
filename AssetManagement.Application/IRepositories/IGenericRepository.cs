using AssetManagement.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement.Application.IRepositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetByCondition(Expression<Func<T, bool>> condition);
        IQueryable<T> GetAllAsync(int? index, int? size);
        Task<int> AddAsync(T entity);
        Task<int> UpdateAsync(T entity);
        Task<int> DeleteAsync(Guid id);
    }
}
