using System;
using DearHome_Backend.Data;
using DearHome_Backend.Repositories.Interfaces;
using DearHome_Backend.Services.Inplementations;
using DearHome_Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DearHome_Backend.Services.Implementations;

public class AttributeService : BaseService<Models.Attribute>, IAttributeService
{
    private readonly IAttributeRepository _repository;
    private readonly IAttributeValueRepository _attributeValueRepository;

    public AttributeService(IAttributeRepository repository, IAttributeValueRepository attributeValueRepository) : base(repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _attributeValueRepository = attributeValueRepository ?? throw new ArgumentNullException(nameof(attributeValueRepository));
    }

    public Task<IEnumerable<Models.Attribute>> GetAllWithAttributeValuesAsync()
    {
        var attributes = _repository.GetAllWithAttributeValuesAsync();
        return attributes;
    }

    public async Task<IEnumerable<Models.Attribute>> GetAllWithCategoryAttributeAsync()
    {
        var attributes = await _repository.GetAllWithCategoryAttributeAsync();
        return attributes;       
    }

    public Task<IEnumerable<Models.Attribute>> GetWithAttributeValuesByCategoryIdAsync(Guid categoryId)
    {
        var attributes = _repository.GetWithAttributeValuesByCategoryIdAsync(categoryId);
        return attributes;
    }

    public override async Task<Models.Attribute> UpdateAsync(Models.Attribute entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        var existingEntity = await _repository.GetByIdWithAttributeValuesAsync(entity.Id);
        if (existingEntity == null)
        {
            throw new KeyNotFoundException($"Attribute with ID {entity.Id} not found.");
        }

        //Track the existing attribute values
        var existingAttributeValues = existingEntity.AttributeValues!.ToList();

        //Track the attribute values that are in the existing entity but not in the new entity
        var attributeValuesToRemove = existingAttributeValues
            .Where(av => !entity.AttributeValues!.Any(newAv => newAv.Value == av.Value))
            .ToList();

        //Remove the existing attribute values that are not in the new list
        foreach (var attributeValue in attributeValuesToRemove)
        {
            await _attributeValueRepository.DeleteAsync(attributeValue.Id);
        }

        //Remove the existing attribute values from the new entity
        entity.AttributeValues = entity.AttributeValues!
            .Where(newAv => !existingAttributeValues.Any(av => av.Value == newAv.Value))
            .ToList();
        
        return await base.UpdateAsync(entity);
    }

}
