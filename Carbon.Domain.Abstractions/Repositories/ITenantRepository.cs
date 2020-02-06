﻿using Carbon.Domain.Abstractions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Carbon.Domain.Abstractions.Repositories
{
    public interface ITenantRepository<T> where T : IMustHaveTenant
    {
        Task<T> GetByIdAsync(Guid id, Guid tenantId);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(Guid id, Guid tenantId);
        Task<List<T>> GetAllAsync(Guid tenantId);


        Task<List<T>> CreateRangeAsync(IEnumerable<T> entities);
        Task<List<T>> UpdateRangeAsync(IEnumerable<T> entities);
        Task<List<T>> DeleteRangeAsync(IEnumerable<T> entities);


        Task<T> Get(Guid tenantId, Expression<Func<T, bool>> predicate);
        IQueryable<T> Query(Guid tenantId);
        IQueryable<T> QueryAsNoTracking(Guid tenantId);
        Task<int> SaveChangesAsync();
    }
}