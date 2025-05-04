using System;

namespace DearHome_Backend.DTOs.PaginationDtos;

public class PaginationFilter
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public PaginationFilter()
    {
        this.PageNumber = 0;    
        this.PageSize = 10;
    }

    public PaginationFilter(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber < 0 ? 0 : pageNumber;
        PageSize = pageSize > 100 ? 100 : pageSize;
    }
}
