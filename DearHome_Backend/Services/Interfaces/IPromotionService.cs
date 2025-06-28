using System;
using DearHome_Backend.DTOs.PaginationDtos;
using DearHome_Backend.Models;

namespace DearHome_Backend.Services.Interfaces;

public interface IPromotionService : IBaseService<Promotion>
{
    Task<PaginatedResult<Promotion>> GetAllAsync(int offSet, int limit, string? search);
    Task<IEnumerable<Promotion>> GetUsablePromotionByUserId(Guid userId);
}
