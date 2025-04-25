using System;
using DearHome_Backend.Data;
using DearHome_Backend.Models;
using DearHome_Backend.Repositories.Interfaces;
using DearHome_Backend.Services.Inplementations;
using DearHome_Backend.Services.Interfaces;

namespace DearHome_Backend.Services.Implementations;

public class CategoryService : BaseService<Category>, ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository) : base(categoryRepository)
    {
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    }
    public Task<Category> GetCategoryByName(string name)
    {
        return _categoryRepository.GetCategoryByName(name);
    }

    public Task<List<Category>> GetCategoriesByParentId(Guid parentId)
    {
        return _categoryRepository.GetCategoriesByParentId(parentId);
    }

    public Task<List<Category>> GetAllWithParentAndAttributes()
    {
        return _categoryRepository.GetAllWithParentAndAttributes();
    }

    public Task<List<Category>> GetAllWithAttributesAndAttributeValues()
    {
        return _categoryRepository.GetAllWithAttributesAndAttributeValues();
    }

    public Task<Category> GetCategoryBySlug(string slug)
    {
        return _categoryRepository.GetCategoryBySlug(slug);
    }
}
