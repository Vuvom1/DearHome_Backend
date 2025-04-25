using System;
using DearHome_Backend.Models;

namespace DearHome_Backend.Services.Interfaces;

public interface IBaseService <T> where T : BaseEntity
{
    Task<T> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}
