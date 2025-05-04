using System;
using DearHome_Backend.DTOs.OrderDtos;
using DearHome_Backend.DTOs.UserDtos;
using DearHome_Backend.DTOs.VariantAttributeDtos;

namespace DearHome_Backend.DTOs.ReviewDtos;

public class ReviewDto
{
    public Guid Id { get; set; }
    public decimal Rating { get; set; }
    public string? ReviewText { get; set; }
    public Guid VariantId { get; set; }
    public virtual VariantDto? Variant { get; set; }
    public Guid UserId { get; set; }
    public virtual UserDto? User { get; set; }
    public Guid OrderId { get; set; }
    public virtual OrderDto? Order { get; set; }
}
