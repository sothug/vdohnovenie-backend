namespace backend.Models;

public class PagedResponse<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
    public T[] Items { get; set; }

    public PagedResponse(List<T> items, int pageNumber, int pageSize, int totalItems)
    {
        Items = items.ToArray();
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalItems = totalItems;
    }
}