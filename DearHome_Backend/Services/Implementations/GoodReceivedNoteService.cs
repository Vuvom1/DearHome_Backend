using System;
using DearHome_Backend.Models;
using DearHome_Backend.Repositories.Interfaces;
using DearHome_Backend.Services.Inplementations;
using DearHome_Backend.Services.Interfaces;

namespace DearHome_Backend.Services.Implementations;

public class GoodReceivedNoteService : BaseService<GoodReceivedNote>, IGoodReceivedNoteService
{
    private readonly IGoodReceivedNoteRepository _goodReceivedNoteRepository;
    private readonly IVariantRepository _variantRepository;

    public GoodReceivedNoteService(IGoodReceivedNoteRepository goodReceivedNoteRepository, IVariantRepository variantRepository) : base(goodReceivedNoteRepository)
    {
        _goodReceivedNoteRepository = goodReceivedNoteRepository;
        _variantRepository = variantRepository;
    }

    public override async Task<GoodReceivedNote> CreateAsync(GoodReceivedNote entity)
    {
        var totalCost = 0m;
        var Quantity = 0;
        if (entity.GoodReceivedItems != null)
        {
            foreach (var item in entity.GoodReceivedItems)
            {
                item.GoodReceivedNoteId = entity.Id;
                item.TotalCost = item.Quantity * item.UnitCost;
                totalCost += item.TotalCost;
                Quantity += item.Quantity;

                await _variantRepository.IncreaseStockAsync(item.VariantId, item.Quantity);
            }
        }

        entity.TotalCost = totalCost;
        entity.Quantity = Quantity;

        return await base.CreateAsync(entity);
    }

    public override async Task<GoodReceivedNote> UpdateAsync(GoodReceivedNote entity)
    {
        var totalCost = 0m;
        var Quantity = 0;
        // Get the existing GoodReceivedNote from the database
        var existingGoodReceivedNote = await _goodReceivedNoteRepository.GetByIdAsync(entity.Id);
        if (existingGoodReceivedNote == null)
        {
            throw new Exception("GoodReceivedNote not found");
        }
        // Check if the GoodReceivedItems have changed

        // If the items have changed, update the stock for the existing items
        foreach (var existingItem in existingGoodReceivedNote.GoodReceivedItems!)
        {
            var newItem = entity.GoodReceivedItems?.FirstOrDefault(i => i.Id == existingItem.Id);
            if (newItem != null)
            {
                // Update the stock for the existing item
                await _variantRepository.IncreaseStockAsync(existingItem.VariantId, newItem.Quantity - existingItem.Quantity);
            }
            else
            {
                // If the item is removed, decrease the stock
                await _variantRepository.IncreaseStockAsync(existingItem.VariantId, -existingItem.Quantity);
            }
        }

        // If there are new items, add them to the stock
        if (entity.GoodReceivedItems != null)
        {            
            foreach (var item in entity.GoodReceivedItems)
            {
                item.GoodReceivedNoteId = entity.Id;
                item.TotalCost = item.Quantity * item.UnitCost;
                totalCost += item.TotalCost;
                Quantity += item.Quantity;
            }
        }

        entity.TotalCost = totalCost;
        entity.Quantity = Quantity;

        return await base.UpdateAsync(entity);
    }
}
