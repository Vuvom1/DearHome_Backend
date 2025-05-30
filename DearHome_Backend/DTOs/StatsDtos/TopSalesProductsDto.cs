using System;
using DearHome_Backend.Models;

namespace DearHome_Backend.DTOs.StatsDtos;

public class TopSalesProductsDto
{
    public class ProductSalesInfo<T>
    {
        public required Product Key { get; set; }
        public required T Value { get; set; }
    }

    public List<ProductSalesInfo<decimal>> TopSaleProductsByAmount { get; set; }
    public List<ProductSalesInfo<int>> TopSaleProductsByCount { get; set; }

    public TopSalesProductsDto()
    {
        TopSaleProductsByAmount = new List<ProductSalesInfo<decimal>>();
        TopSaleProductsByCount = new List<ProductSalesInfo<int>>();
    }
    
}
