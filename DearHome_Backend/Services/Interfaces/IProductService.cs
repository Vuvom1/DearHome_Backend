using System;
using DearHome_Backend.Models;

namespace DearHome_Backend.Services.Interfaces;

public interface IProductService : IBaseService<Product>
{
    Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid id);
    Task<IEnumerable<Product>> GetTopSalesProductsAsync(int count);
    Task<IEnumerable<Product>> GetAllWithVariantsAsync();
    Task<Product?> GetByIdWithAttributeValuesAndVariantsAsync(Guid id);
}
