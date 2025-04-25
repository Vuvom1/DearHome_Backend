using System;
using DearHome_Backend.Models;
using DearHome_Backend.Repositories.Interfaces;
using DearHome_Backend.Services.Inplementations;
using DearHome_Backend.Services.Interfaces;

namespace DearHome_Backend.Services.Implementations;

public class PlacementService : BaseService<Placement>, IPlacementService
{
    private readonly IPlacementRepository _placementRepository;

    public PlacementService(IPlacementRepository placementRepository) : base(placementRepository)
    {
        _placementRepository = placementRepository;
    }
}
