using System;
using DearHome_Backend.DTOs.PaginationDtos;
using DearHome_Backend.Models;

namespace DearHome_Backend.Services.Interfaces;

public interface IBaseService<T> where T : BaseEntity
{
    Task<T> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<PaginatedResult<T>> GetAllAsync(int offSet, int limit, string? search = null);
    Task<PaginatedResult<T>> GetAllAsync(int offSet, int limit, string? search = null, string? filter = null, string? sortBy = null, bool isDescending = false);
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}
