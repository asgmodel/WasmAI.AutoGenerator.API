namespace AutoGenerator;

public abstract class PagedResponseBase
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

public class PagedResponse<T> : PagedResponseBase
{
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }
    public IEnumerable<T> Data { get; set; }

    public PagedResponse(IEnumerable<T> data, int pageNumber, int pageSize, int totalRecords)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalRecords = totalRecords;
        TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        Data = data;
    }

    public PagedResponse(IEnumerable<T> data, int pageNumber, int pageSize, int totalRecords, string? sortBy, string? sortDirection)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalRecords = totalRecords;
        TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        Data = data;
        SortBy = sortBy;
        SortDirection = sortDirection;
    }

    public PagedResponse<T2> ToResponse<T2>(IEnumerable<T2> data)
    {
        var response = new PagedResponse<T2>(data, PageNumber, PageSize, TotalRecords, SortBy, SortDirection);
        response.TotalPages = TotalPages;
        return response;
    }
}

