using System;
using DearHome_Backend.Models;
using DearHome_Backend.Repositories.Interfaces;
using DearHome_Backend.Services.Inplementations;
using DearHome_Backend.Services.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DearHome_Backend.Services.Implementations;

public class VariantService : BaseService<Variant>, IVariantService
{
    private readonly IVariantRepository _variantRepository;
    private readonly IVariantAttributeRepository _variantAttributeRepository;
    private readonly IProductRepository _productRepository;
    private readonly NatsService _natsService;
    private readonly JsonSerializerOptions _jsonOptions;

    public VariantService(IVariantRepository variantRepository, 
        IVariantAttributeRepository variantAttributeRepository, 
        IProductRepository productRepository, 
        NatsService natsService) : base(variantRepository)
    {
        _variantRepository = variantRepository;
        _variantAttributeRepository = variantAttributeRepository ?? throw new ArgumentNullException(nameof(variantAttributeRepository));
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _natsService = natsService ?? throw new ArgumentNullException(nameof(natsService));
        
        _jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            MaxDepth = 64,
            WriteIndented = true
        };
    }

    public async Task<IEnumerable<Variant>> GetByProductIdAsync(Guid productId)
    {
        return await _variantRepository.GetByProductIdAsync(productId);
    }

    public override async Task<Variant> CreateAsync(Variant entity)
    {

        var createdVariant = await base.CreateAsync(entity);

        await PublishVariantCreatedEventAsync(createdVariant);

        return createdVariant;
    }

    public override async Task<Variant> UpdateAsync(Variant entity)
    {
        var existingVariant = await _variantRepository.GetByIdWithVariantAttributesAsync(entity.Id);

        //Remove all existing attribute values
        if (existingVariant?.VariantAttributes != null)
        {
            var existingVariantAttributeIds = existingVariant.VariantAttributes.Select(v => v.Id).ToList();
            foreach (var attributeValueId in existingVariantAttributeIds)
            {
                await _variantAttributeRepository.DeleteAsync(attributeValueId);
            }
        }

        await PublishVariantUpdatedEventAsync(entity);

        return await base.UpdateAsync(entity);
    }

    public override async Task DeleteAsync(Guid id)
    {
        await base.DeleteAsync(id);

        PublishVariantDeletedEvent(id);
    }

    private async Task PublishVariantCreatedEventAsync(Variant createdVariant)
    {
        var product = await _productRepository.GetByIdWithAttributeValuesAndVariantsAsync(createdVariant.ProductId);

        var attributes = await _variantAttributeRepository.GetWithAttributeValuesByIdAsync(createdVariant.VariantAttributes?.Select(attr => attr.Id) ?? Enumerable.Empty<Guid>());
        createdVariant.VariantAttributes = attributes.Select(attr => new VariantAttribute
        {
            Id = attr.Id,
            AttributeValueId = attr.AttributeValueId,
            VariantId = createdVariant.Id,
            AttributeValue = attr.AttributeValue
        }).ToList();

        var variantData = new{
            id = createdVariant.Id,
            product = new
            {
                id = product?.Id,
                name = product?.Name,
                description = product?.Description,
                category = product?.Category?.Name,
                placement = product?.Placement?.Name,
            },
            attributes = createdVariant.VariantAttributes?.Select(attr => new
            {
                id = attr.AttributeValueId,
                value = attr.AttributeValue?.Value
            }),
            sku = createdVariant.Sku,
            price_adjustment = createdVariant.PriceAdjustment,
            stock_quantity = createdVariant.Stock,
            is_active = createdVariant.IsActive,
            created_at = createdVariant.CreatedAt
        };

        var publishedData = new
        {
            operation = "create",
            variant = variantData
        };

        string jsonData = JsonSerializer.Serialize(publishedData, _jsonOptions);
        _natsService.Publish("variant.sync", jsonData);
    }

    private async Task PublishVariantUpdatedEventAsync(Variant updatedVariant)
    {
        var product = await _productRepository.GetByIdWithAttributeValuesAndVariantsAsync(updatedVariant.ProductId);

        var attributes = await _variantAttributeRepository.GetWithAttributeValuesByIdAsync(updatedVariant.VariantAttributes?.Select(attr => attr.Id) ?? Enumerable.Empty<Guid>());
        updatedVariant.VariantAttributes = attributes.Select(attr => new VariantAttribute
        {
            Id = attr.Id,
            AttributeValueId = attr.AttributeValueId,
            VariantId = updatedVariant.Id,
            AttributeValue = attr.AttributeValue
        }).ToList();

        var variantData = new{
            id = updatedVariant.Id,
            product = new
            {
                id = product?.Id,
                name = product?.Name,
                description = product?.Description,
                category = product?.Category?.Name,
                placement = product?.Placement?.Name,
            },
            attributes = updatedVariant.VariantAttributes?.Select(attr => new
            {
                id = attr.AttributeValueId,
                value = attr.AttributeValue?.Value
            }),
            sku = updatedVariant.Sku,
            price_adjustment = updatedVariant.PriceAdjustment,
            stock_quantity = updatedVariant.Stock,
            is_active = updatedVariant.IsActive,
            created_at = updatedVariant.CreatedAt
        };

        var publishedData = new
        {
            operation = "update",
            variant = variantData
        };

        string jsonData = JsonSerializer.Serialize(publishedData, _jsonOptions);
        _natsService.Publish("variant.sync", jsonData);
    }

    private void PublishVariantDeletedEvent(Guid variantId)
    {
        var publishedData = new
        {
            operation = "delete",
            variant = new
            {
                id = variantId
            }
        };

        string jsonData = JsonSerializer.Serialize(publishedData, _jsonOptions);
        _natsService.Publish("variant.sync", jsonData);
    }


}
