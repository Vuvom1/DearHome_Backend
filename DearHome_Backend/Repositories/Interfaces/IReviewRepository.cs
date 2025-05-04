using System;
using DearHome_Backend.DTOs.PaginationDtos;
using DearHome_Backend.DTOs.ReviewDtos;
using DearHome_Backend.Models;

namespace DearHome_Backend.Repositories.Interfaces;

public interface IReviewRepository : IBaseRepository<Review>
{
    Task<PaginatedResponse<IEnumerable<Review>>> GetByProductIdAsync(Guid productId, int pageNumber, int pageSize, string sortBy = "createdAt", bool isDescending = true);
    Task<IEnumerable<Review?>> GetByOrderIdAsync(Guid orderId);  
}
