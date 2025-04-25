using System;
using DearHome_Backend.Models;

namespace DearHome_Backend.Repositories.Interfaces;

public interface IVariantRepository : IBaseRepository<Variant>
{
    Task<List<Variant>> GetByProductId(Guid productId);
    Task<Variant?> GetByIdWithVariantAttributesAsync(Guid id);
    Task IncreaseStockAsync(Guid variantId, int quantity);
    Task UpdateStockAsync(Guid variantId, int quantity);
}
