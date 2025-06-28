using System;
using System.Collections;
using DearHome_Backend.DTOs.PaginationDtos;
using DearHome_Backend.Models;

namespace DearHome_Backend.DTOs.ReviewDtos;

public class ProductReviewsDto
{
    public Guid ProductId { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public Dictionary<decimal, int> RatingCounts { get; set; } = new Dictionary<decimal, int>();
    public  PaginatedResult<Review> Reviews { get; set; } = new PaginatedResult<Review>();
}
