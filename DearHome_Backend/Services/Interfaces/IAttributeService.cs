using System;


namespace DearHome_Backend.Services.Interfaces;

public interface IAttributeService : IBaseService<Models.Attribute>
{
    Task<IEnumerable<Models.Attribute>> GetAllWithCategoryAttributeAsync();
    Task<IEnumerable<Models.Attribute>> GetWithAttributeValuesByCategoryIdAsync(Guid categoryId);
    Task<IEnumerable<Models.Attribute>> GetAllWithAttributeValuesAsync();
}
