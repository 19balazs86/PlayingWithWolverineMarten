using System.Diagnostics;

namespace WolverineHttpWebAPI.Pagination;

[DebuggerDisplay("Page = {Page}, PageCount = {PageCount}, TotalCount = {TotalCount}, IsEmpty = {IsEmpty}")]
public class PageResult<TEntity>
{
    public IEnumerable<TEntity> Items { get; private set; }

    public int Page { get; private set; }
    public int PageSize { get; private set; }
    public int PageCount { get; private set; }
    public long TotalCount { get; private set; }

    public bool IsEmpty => Items == null || !Items.Any();
    public bool IsNotEmpty => !IsEmpty;
    public bool HasNextPage => Page < PageCount;

    public static PageResult<TEntity> Empty => new PageResult<TEntity>();

    public PageResult()
    {
        Items = Enumerable.Empty<TEntity>();
    }

    public PageResult(IEnumerable<TEntity> items, int page, int pageSize, int pageCount, long totalCount)
    {
        Page       = page > pageCount ? pageCount : page;
        PageSize   = pageSize;
        PageCount  = pageCount;
        TotalCount = totalCount;

        Items = items;
    }
}