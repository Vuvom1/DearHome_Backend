using System;

namespace DearHome_Backend.Repositories.Interfaces;

public interface IAttributeRepository : IBaseRepository<Models.Attribute>
{
    Task<IEnumerable<Models.Attribute>> GetAllWithCategoryAttributeAsync();
    Task<IEnumerable<Models.Attribute>> GetAllWithAttributeValuesAsync();
    Task<IEnumerable<Models.Attribute>> GetWithAttributeValuesByCategoryIdAsync(Guid categoryId);
    Task<Models.Attribute?> GetByIdWithAttributeValuesAsync(Guid id);
}
