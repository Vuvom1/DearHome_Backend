using System;
using DearHome_Backend.Models;

namespace DearHome_Backend.Repositories.Interfaces;

public interface IVariantRepository : IBaseRepository<Variant>
{
    Task<IEnumerable<Variant>> GetByProductIdAsync(Guid productId);
    Task<Variant?> GetWithProductByIdAsync(Guid productId);
    Task<Variant?> GetByIdWithVariantAttributesAsync(Guid id);
    Task IncreaseStockAsync(Guid variantId, int quantity);
    Task UpdateStockAsync(Guid variantId, int quantity);
}
