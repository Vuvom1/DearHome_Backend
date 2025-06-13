using System;
using DearHome_Backend.Models;

namespace DearHome_Backend.Repositories.Interfaces;

public interface IVariantAttributeRepository : IBaseRepository<VariantAttribute>
{
    Task<IEnumerable<VariantAttribute>> GetWithAttributeValuesByIdAsync(IEnumerable<Guid> ids);
}
