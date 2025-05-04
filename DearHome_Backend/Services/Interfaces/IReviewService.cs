using System;
using DearHome_Backend.DTOs.PaginationDtos;
using DearHome_Backend.DTOs.ReviewDtos;
using DearHome_Backend.Models;

namespace DearHome_Backend.Services.Interfaces;

public interface IReviewService : IBaseService<Review>  
{
    Task<PaginatedResponse<IEnumerable<Review>>> GetByProductIdAsync(Guid productId, int page = 1, int pageSize = 10, string? sortBy = null, bool isDescending = false);
    Task<IEnumerable<Review>> GetByOrderIdAsync(Guid orderId);
    Task<ProductReviewsDto> GetProductReviewsAsync(Guid productId, int page = 1, int pageSize = 10, string? sortBy = null, bool isDescending = false);
}
