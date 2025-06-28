using System;
using DearHome_Backend.Models;
using DearHome_Backend.Repositories.Interfaces;
using DearHome_Backend.Services.Inplementations;
using DearHome_Backend.Services.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;
using DearHome_Backend.Constants;

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
        await PublishVariantUpdatedEventAsync(entity);

        // Add variate attributes if they are not already present
        if (entity.VariantAttributes != null && entity.VariantAttributes.Any())
        {
            foreach (var variantAttribute in entity.VariantAttributes)
            {
                if (variantAttribute.Id == Guid.Empty)
                {
                    // If the attribute does not have an ID, it is a new attribute
                    variantAttribute.VariantId = entity.Id; // Set the VariantId for the new attribute
                    await _variantAttributeRepository.AddAsync(variantAttribute);
                }
                else
                {
                    // If the attribute already exists, update it
                    var existingAttribute = await _variantAttributeRepository.GetByIdAsync(variantAttribute.Id);
                    if (existingAttribute != null)
                    {
                        existingAttribute.AttributeValueId = variantAttribute.AttributeValueId;
                        await _variantAttributeRepository.UpdateAsync(existingAttribute);
                    }
                }
            }
        }

        return await base.UpdateAsync(entity);
    }

    public override async Task DeleteAsync(Guid id)
    {
        await base.DeleteAsync(id);

        PublishVariantDeletedEvent(id);
    }
    
    private void PublishEvent(string subject, object message)
    {
        var jsonMessage = JsonSerializer.Serialize(message, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });
        _natsService.Publish(subject, jsonMessage);
    }

    private async Task<object> CreateVariantEventData(Variant variant)
    {
        var product = await _productRepository.GetByIdWithAttributeValuesAndVariantsAsync(variant.ProductId);

        var attributes = await _variantAttributeRepository.GetWithAttributeValuesByIdAsync(variant.VariantAttributes?.Select(attr => attr.Id) ?? Enumerable.Empty<Guid>());
        variant.VariantAttributes = attributes.Select(attr => new VariantAttribute
        {
            Id = attr.Id,
            AttributeValueId = attr.AttributeValueId,
            VariantId = variant.Id,
            AttributeValue = attr.AttributeValue
        }).ToList();

        return new
        {
            id = variant.Id,
            product = new
            {
                id = product?.Id,
                name = product?.Name,
                description = product?.Description,
                category = product?.Category?.Name,
                placement = product?.Placement?.Name,
            },
            attributes = variant.VariantAttributes?.Select(attr => new
            {
                id = attr.AttributeValueId,
                value = attr.AttributeValue?.Value
            }),
            sku = variant.Sku,
            price_adjustment = variant.PriceAdjustment,
            stock_quantity = variant.Stock,
            is_active = variant.IsActive,
            created_at = variant.CreatedAt
        };
    }

    private async Task PublishVariantCreatedEventAsync(Variant createdVariant)
    {
        var variantData = await CreateVariantEventData(createdVariant);

        _natsService.Publish(NatsConstants.VariantCreated, variantData);
    }

    private async Task PublishVariantUpdatedEventAsync(Variant updatedVariant)
    {
        var variantData = await CreateVariantEventData(updatedVariant);

        _natsService.Publish(NatsConstants.VariantUpdated, variantData);
    }

    private void PublishVariantDeletedEvent(Guid variantId)
    {
        _natsService.Publish(NatsConstants.VariantDeleted, new { id = variantId });
    }


}
