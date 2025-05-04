using System;

namespace DearHome_Backend.DTOs.ReviewDtos;

public class AddReviewDto
{
     public decimal Rating { get; set; }
    public string? ReviewText { get; set; }
    public Guid OrderDetailId { get; set; }
}
