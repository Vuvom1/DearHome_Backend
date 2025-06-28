using System;
using DearHome_Backend.DTOs.PaginationDtos;
using DearHome_Backend.DTOs.ReviewDtos;
using DearHome_Backend.Models;
using DearHome_Backend.Repositories.Interfaces;
using DearHome_Backend.Services.Inplementations;
using DearHome_Backend.Services.Interfaces;

namespace DearHome_Backend.Services.Implementations;

public class ReviewService : BaseService<Review>, IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    public ReviewService(IReviewRepository reviewRepository) : base(reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<IEnumerable<Review>> GetByOrderIdAsync(Guid orderId)
    {
        var reviews = await _reviewRepository.GetByOrderIdAsync(orderId);
        return reviews.Where(review => review != null)!;
    }

    public async Task<PaginatedResult<Review>> GetByProductIdAsync(Guid productId, int page, int pageSize, string? sortBy, bool isDescending)
    {
        return await _reviewRepository.GetByProductIdAsync(productId, page, pageSize, sortBy ?? "createdAt", isDescending);
    }

    public async Task<ProductReviewsDto> GetProductReviewsAsync(Guid productId, int page = 1, int pageSize = 10, string? sortBy = null, bool isDescending = false)
    {
        var paginatedReviews = await GetByProductIdAsync(productId, page, pageSize, sortBy, isDescending);
        var reviews = paginatedReviews.Data.ToList();
        var totalCount = paginatedReviews.TotalRecords;

        return new ProductReviewsDto
        {
            Reviews = paginatedReviews,    
            AverageRating = reviews.Count > 0 ? reviews.Average(r => r.Rating) : 0,
            TotalReviews = reviews.Count,
            RatingCounts = new Dictionary<decimal, int>()
            {
                { 1m, reviews.Count(r => r.Rating == 1) },
                { 2m, reviews.Count(r => r.Rating == 2) },
                { 3m, reviews.Count(r => r.Rating == 3) },
                { 4m, reviews.Count(r => r.Rating == 4) },
                { 5m, reviews.Count(r => r.Rating == 5) }
            }
        };
    }
}
