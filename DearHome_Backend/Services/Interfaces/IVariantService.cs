using System;
using DearHome_Backend.Models;

namespace DearHome_Backend.Services.Interfaces;

public interface IVariantService : IBaseService<Variant>
{
    Task<IEnumerable<Variant>> GetByProductIdAsync(Guid productId);
}
