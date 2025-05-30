using System;

namespace DearHome_Backend.DTOs.StatsDtos;

public class CategoryWithStockAndPercentage
{
    public required Guid Id { get; set; }
    public required string CategoryName { get; set; }
    public required int StockAmount { get; set; }
    public required decimal Percentage { get; set; }
    public IEnumerable<SubCategoryStock> SubCategories { get; set; } = new List<SubCategoryStock>();
}

public class SubCategoryStock
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required int StockAmount { get; set; }
    public required decimal Percentage { get; set; }
}
