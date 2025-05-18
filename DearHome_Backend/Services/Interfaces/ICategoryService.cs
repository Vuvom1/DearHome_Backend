using System;
using DearHome_Backend.Models;

namespace DearHome_Backend.Services.Interfaces;

public interface ICategoryService : IBaseService<Category>
{
    Task<Category> GetCategoryByName(string name);
    Task<IEnumerable<Category>> GetAllParentCategories();
    Task<Category> GetCategoryBySlug(string slug);
    Task<List<Category>> GetAllWithParentAndAttributes();
    Task<List<Category>> GetAllWithAttributesAndAttributeValues();
    Task<List<Category>> GetCategoriesByParentId(Guid parentId);
}
