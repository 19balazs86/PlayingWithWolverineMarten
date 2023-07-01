using System.Diagnostics;

namespace WolverineHttpWebAPI.Pagination;

[DebuggerDisplay("Page = {Page}, PageCount = {PageCount}, TotalCount = {TotalCount}, IsEmpty = {IsEmpty}")]
public class PageResult<TEntity>
{
    public IEnumerable<TEntity> Items { get; init; }

    public int Page { get; init; }
    public int PageSize { get; init; }
    public int PageCount { get; init; }
    public long TotalCount { get; init; }

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