using System;
using System.Linq.Expressions;
using DearHome_Backend.DTOs.PaginationDtos;

namespace DearHome_Backend.Repositories.Interfaces;

public interface IBaseRepository <T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<PaginatedResult<T>> GetAllAsync(int offSet, int limit, string? search = null);
    Task<PaginatedResult<T>> GetAllAsync(int offSet, int limit, string? search = null, string? filter = null, string? sortBy = null, bool isDescending = false);
    Task<T?> GetByIdAsync(object id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(object id);
    Task DeleteRangeAsync(IEnumerable<T> entities);
    Task DeleteRangeByIdsAsync(IEnumerable<object> ids);
    IQueryable<T> Find(Expression<Func<T, bool>> predicate);
}
