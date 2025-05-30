using System;
using DearHome_Backend.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DearHome_Backend.Repositories.Interfaces;

public interface IProductRepository : IBaseRepository<Product>
{
    Task<IEnumerable<Product?>> GetByCategoryIdAsync(Guid id);
    Task<IEnumerable<KeyValuePair<Product, decimal>>> GetTopSalesProductsWithAmountsAsync(DateTime startDate, DateTime endDate, int count);
    Task<IEnumerable<KeyValuePair<Product, int>>> GetTopSalesProductsWithCountsAsync(DateTime startDate, DateTime endDate, int count);
    Task<int> GetTotalProductsCountAsync();
    Task<IEnumerable<Product>> GetTopSalesProductsAsync(int count);
    Task<Product?> GetByIdWithAttributeValuesAndVariantsAsync(Guid id);
    Task<IEnumerable<Product>> GetAllWithVariantsAsync();
}
