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

        // Update the existing entity's properties
        // Remove all old attribute values
        if (existingEntity.AttributeValues != null)
        {
            var attributeValueIds = existingEntity.AttributeValues.Select(av => av.Id).ToList();
            foreach (var id in attributeValueIds)
            {
                await _attributeValueRepository.DeleteAsync(id);
            }
        }
        
        return await base.UpdateAsync(entity);
    }

}
