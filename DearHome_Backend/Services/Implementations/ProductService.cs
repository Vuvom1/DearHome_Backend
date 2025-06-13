using System;
using DearHome_Backend.Models;
using DearHome_Backend.Repositories.Interfaces;
using DearHome_Backend.Services.Inplementations;
using DearHome_Backend.Services.Interfaces;

namespace DearHome_Backend.Services.Implementations;

public class ProductService : BaseService<Product>, IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly NatsService _natsService;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IPlacementRepository _placementRepository;

    public ProductService(IProductRepository productRepository, NatsService natsService, ICategoryRepository categoryRepository, IPlacementRepository placementRepository) : base(productRepository)
    {
        _productRepository = productRepository;
        _natsService = natsService;
        _categoryRepository = categoryRepository;
        _placementRepository = placementRepository;
    }

    public Task<IEnumerable<Product>> GetAllWithVariantsAsync()
    {
        return _productRepository.GetAllWithVariantsAsync();
    }

    public async Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid id)
    {
        var products = await _productRepository.GetByCategoryIdAsync(id);
        return products.Where(product => product != null)!;
    }

    public override async Task<Product> GetByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdWithAttributeValuesAndVariantsAsync(id);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product with ID {id} was not found.");
        }
        return product;
    }

    public Task<Product?> GetByIdWithAttributeValuesAndVariantsAsync(Guid id)
    {
        return _productRepository.GetByIdWithAttributeValuesAndVariantsAsync(id);
    }

    public async Task<IEnumerable<Product>> GetTopSalesProductsAsync(int count)
    {
        return await _productRepository.GetTopSalesProductsAsync(count);
    }

    public override async Task<Product> CreateAsync(Product entity)
    {
        var createdProduct = await base.CreateAsync(entity);

        var categoryName = await _categoryRepository.GetCategoryNameByIdAsync(createdProduct.CategoryId);
        var placementName = await _placementRepository.GetPlacementNameByIdAsync(createdProduct.PlacementId);

        var publishedData = new
        {
            operation = "create",
            product = new
            {
                id = createdProduct.Id,
                name = createdProduct.Name,
                description = createdProduct.Description,
                price = createdProduct.Price,
                category = categoryName,
                placement = placementName,
                is_active = createdProduct.IsActive,
                created_at = DateTime.UtcNow,
            }
        };

        // Publish notification about the new product
        _natsService.Publish("product.sync",
            System.Text.Json.JsonSerializer.Serialize(publishedData));

        return createdProduct;
    }

    public override async Task<Product> UpdateAsync(Product entity)
    {
        var updatedProduct = await base.UpdateAsync(entity);

        var categoryName = await _categoryRepository.GetCategoryNameByIdAsync(updatedProduct.CategoryId);
        var placementName = await _placementRepository.GetPlacementNameByIdAsync(updatedProduct.PlacementId);

        var publishedData = new
        {
            operation = "update",
            product = new
            {
                id = updatedProduct.Id,
                name = updatedProduct.Name,
                description = updatedProduct.Description,
                price = updatedProduct.Price,
                category = categoryName,
                placement = placementName,
                is_active = updatedProduct.IsActive,
                update_at = DateTime.UtcNow
            }
        };

        // Publish notification about the updated product
        _natsService.Publish("product.sync",
            System.Text.Json.JsonSerializer.Serialize(publishedData));

        return await base.UpdateAsync(entity);
    }

    public override async Task DeleteAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product with ID {id} was not found.");
        }

        await base.DeleteAsync(id);
        var publishedData = new
        {
            operation = "delete",
            product = new
            {
                id = product.Id,
            }
        };

        // Publish notification about the deleted product
        _natsService.Publish("product.sync",
            System.Text.Json.JsonSerializer.Serialize(publishedData));
    }
}
