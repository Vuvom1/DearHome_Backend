using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DearHome_Backend.DTOs.PaginationDtos;
using DearHome_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DearHome_Backend.Repositories.Implementations
{
    /// <summary>
    /// Base repository class that implements common CRUD operations.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <remarks>
    /// This class provides a generic implementation for basic CRUD operations.
    /// </remarks>
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly DbContext _context;

        public BaseRepository(DbContext context)
        {
            _context = context;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public virtual async Task<PaginatedResult<T>> GetAllAsync(int offSet, int limit, string? search = null)
        {
            var query = _context.Set<T>().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                // Assuming T has a property called "Name" for search purposes
                query = query.Where(e => EF.Property<string>(e, "Name").Contains(search));
            }

            return new PaginatedResult<T>
            {
                Data = await query.Skip(offSet).Take(limit).ToListAsync(),
                PageNumber = offSet / limit + 1,
                PageSize = limit,
                TotalRecords = await query.CountAsync()
            };
        }

        public virtual async Task<PaginatedResult<T>> GetAllAsync(int offSet, int limit, string? search = null, string? filter = null, string? sortBy = null, bool isDescending = false)
        {
            var query = _context.Set<T>().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(e => EF.Property<string>(e, "Name").Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(e => EF.Property<string>(e, filter) != null);
            }

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                query = isDescending ? query.OrderByDescending(e => EF.Property<object>(e, sortBy)) : query.OrderBy(e => EF.Property<object>(e, sortBy));
            }

            return new PaginatedResult<T>
            {
                Data = await query.Skip(offSet).Take(limit).ToListAsync(),
                PageNumber = offSet / limit + 1,
                PageSize = limit,
                TotalRecords = await query.CountAsync()
            };
        }
        

        public virtual async Task<T?> GetByIdAsync(object id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public virtual async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(T entity)
        {
            var trackedEntity = _context.Set<T>().Local.FirstOrDefault(e =>
            {
                var eId = _context.Entry(e).Property("Id").CurrentValue;
                var entityId = _context.Entry(entity).Property("Id").CurrentValue;
                return eId != null && entityId != null && eId.Equals(entityId);
            });
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(object id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public IQueryable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate);
        }

        public Task DeleteRangeAsync(IEnumerable<T> entities)
        {
           return Task.Run(() =>
            {
                _context.Set<T>().RemoveRange(entities);
                _context.SaveChanges();
            });
        }

        public Task DeleteRangeByIdsAsync(IEnumerable<object> ids)
        {
            return Task.Run(() =>
            {
                var entities = _context.Set<T>().Where(e => ids.Contains(_context.Entry(e).Property("Id").CurrentValue));
                _context.Set<T>().RemoveRange(entities);
                _context.SaveChanges();
            });
        }
    }
}
