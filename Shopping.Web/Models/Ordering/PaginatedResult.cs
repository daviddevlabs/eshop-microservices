namespace Shopping.Web.Models.Ordering;

public class PaginatedResult<TEntity>
    (int pageIndex, int pageSize, long count, int totalPages, IEnumerable<TEntity> data)
    where TEntity : class
{
    public int PageIndex { get; } = pageIndex;
    public int PageSize { get; } = pageSize;
    public long Count { get; } = count;
    public int TotalPages { get; set; } = totalPages;
    public IEnumerable<TEntity> Data { get; } = data;
}