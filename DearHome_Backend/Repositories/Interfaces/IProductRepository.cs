using System;
using DearHome_Backend.Models;

namespace DearHome_Backend.Repositories.Interfaces;

public interface IProductRepository : IBaseRepository<Product>
{
    Task<IEnumerable<Product?>> GetByCategoryIdAsync(Guid id);
    Task<Product?> GetByIdWithAttributeValuesAndVariantsAsync(Guid id);
    Task<IEnumerable<Product>> GetAllWithVariantsAsync();
}
