using System;

namespace DearHome_Backend.DTOs.PaginationDtos;

public class PaginatedResult<T>
{
    public List<T> Data { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }   
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public bool HasPreviousPage => PageNumber > 0;
    public bool HasNextPage => PageNumber < TotalPages;

    public PaginatedResult(List<T> data, int pageNumber, int pageSize, int totalRecords)
    {
        this.Data = data ?? throw new ArgumentNullException(nameof(data));
        this.PageNumber = pageNumber;
        this.PageSize = pageSize;
        this.TotalRecords = totalRecords;
        this.TotalPages = totalRecords > 0 
            ? (int)Math.Ceiling((double)totalRecords / pageSize) 
            : 0;
    }

    public PaginatedResult()
    {
        Data = default!;
        PageNumber = 0;
        PageSize = 0;
        TotalPages = 0;
        TotalRecords = 0;
    }
}
