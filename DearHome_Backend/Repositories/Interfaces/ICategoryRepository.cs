using System;
using DearHome_Backend.DTOs.StatsDtos;
using DearHome_Backend.Models;

namespace DearHome_Backend.Repositories.Interfaces;

public interface ICategoryRepository : IBaseRepository<Category>
{
    Task<Category> GetCategoryByName(string name);
    Task<IEnumerable<Category>> GetAllParentCategories();
    Task<Category> GetCategoryBySlug(string slug);
    Task<List<Category>> GetAllWithParentAndAttributes();
    Task<List<Category>> GetCategoriesByParentId(Guid parentId);
    Task<List<Category>> GetAllWithAttributesAndAttributeValues();
    Task<IEnumerable<CategoryWithStockAndPercentage>> GetCategoriesWithTotalStockAndPercentageAsync();
}