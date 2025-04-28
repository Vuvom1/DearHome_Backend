using System;
using DearHome_Backend.Models;
using DearHome_Backend.Repositories.Interfaces;
using DearHome_Backend.Services.Inplementations;
using DearHome_Backend.Services.Interfaces;

namespace DearHome_Backend.Services.Implementations;

public class VariantService : BaseService<Variant>, IVariantService
{
    private readonly IVariantRepository _variantRepository;
    private readonly IVariantAttributeRepository _variantAttributeRepository;

    public VariantService(IVariantRepository variantRepository, IVariantAttributeRepository variantAttributeRepository) : base(variantRepository)
    {
        _variantRepository = variantRepository;
        _variantAttributeRepository = variantAttributeRepository ?? throw new ArgumentNullException(nameof(variantAttributeRepository));
    }

    public async Task<IEnumerable<Variant>> GetByProductIdAsync(Guid productId)
    {
        return await _variantRepository.GetByProductIdAsync(productId);
    }

    public override async Task<Variant> UpdateAsync(Variant entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        var existingVariant = await _variantRepository.GetByIdWithVariantAttributesAsync(entity.Id);
        if (existingVariant == null)
        {
            throw new KeyNotFoundException($"Variant with ID {entity.Id} not found.");
        }


        //Remove all existing attribute values
        if (existingVariant.VariantAttributes != null)
        {
            var existingVariantAttributeIds = existingVariant.VariantAttributes.Select(v => v.Id).ToList();
            foreach (var attributeValueId in existingVariantAttributeIds)
            {
                await _variantAttributeRepository.DeleteAsync(attributeValueId);
            }
        }

        return await base.UpdateAsync(entity);
    }
}
