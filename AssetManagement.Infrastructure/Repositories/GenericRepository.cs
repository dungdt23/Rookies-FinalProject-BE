﻿using AssetManagement.Application.IRepositories;
using AssetManagement.Domain.Common;
using AssetManagement.Domain.Constants;
using AssetManagement.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AssetManagement.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly AssetManagementDBContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AssetManagementDBContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }


        public IQueryable<T> GetAllAsync(int? index, int? size)
        {
            IQueryable<T> query = _dbSet.Where(x => !x.IsDeleted);
            if (index.HasValue && size.HasValue)
            {
                query = query.Skip((index.Value - 1) * size.Value).Take(size.Value);
            }
            return query;
        }

        public async Task<int> AddAsync(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
                int status = await _context.SaveChangesAsync();
                return status;
            }
            catch (Exception)
            {
                return RecordStatus.Invalid;
            }
        }

        public async Task<int> UpdateAsync(T entity)
        {
            try
            {
                entity.UpdatedAt = DateTime.Now;
                _dbSet.Update(entity);
                int status = await _context.SaveChangesAsync();
                return status;
            }
            catch (Exception)
            {
                return RecordStatus.Invalid;
            }
        }

        public virtual async Task<int> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null) return RecordStatus.Invalid;
                entity.DeletedAt = DateTime.Now;
                entity.IsDeleted = true;
                _dbSet.Update(entity);
                int status = await _context.SaveChangesAsync();
                return status;
            }
            catch (Exception)
            {
                return RecordStatus.Invalid;
            }
        }

        public IQueryable<T> GetByCondition(Expression<Func<T, bool>> condition)
        {
            return _dbSet.Where(condition);
        }

    }
}
