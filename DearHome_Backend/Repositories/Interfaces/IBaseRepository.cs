using System;
using System.Linq.Expressions;

namespace DearHome_Backend.Repositories.Interfaces;

public interface IBaseRepository <T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(object id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(object id);
    Task DeleteRangeAsync(IEnumerable<T> entities);
    Task DeleteRangeByIdsAsync(IEnumerable<object> ids);
    IQueryable<T> Find(Expression<Func<T, bool>> predicate);
}
