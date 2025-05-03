using System;
using DearHome_Backend.Constants;
using DearHome_Backend.Models;

namespace DearHome_Backend.Repositories.Interfaces;

public interface IPromotionRepository : IBaseRepository<Promotion>
{
    Task<IEnumerable<Promotion>> GetAllAsync(int offSet, int limit, string? search);
    Task<IEnumerable<Promotion>> GetUsablePromotionByCustomterLeverl(CustomerLevels customerLevel);
}
