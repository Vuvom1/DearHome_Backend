using System;
using DearHome_Backend.Models;
using DearHome_Backend.Repositories.Interfaces;
using DearHome_Backend.Services.Inplementations;
using DearHome_Backend.Services.Interfaces;

namespace DearHome_Backend.Services.Implementations;

public class ProductService : BaseService<Product>, IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository) : base(productRepository)
    {
        _productRepository = productRepository;
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
}
