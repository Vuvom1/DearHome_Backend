using System;
using DearHome_Backend.Models;

namespace DearHome_Backend.Repositories.Interfaces;

public interface IPlacementRepository : IBaseRepository<Placement>
{
    Task<string> GetPlacementNameByIdAsync(Guid id);
}
