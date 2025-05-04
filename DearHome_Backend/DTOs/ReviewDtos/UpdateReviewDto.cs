using System;

namespace DearHome_Backend.DTOs.ReviewDtos;

public class UpdateReviewDto
{
    public Guid Id { get; set; }
    public decimal Rating { get; set; }
    public string? ReviewText { get; set; }
    public Guid OrderDetailId { get; set; }
}
