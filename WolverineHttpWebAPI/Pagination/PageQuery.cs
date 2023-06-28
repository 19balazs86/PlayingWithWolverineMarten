using System.Diagnostics;
using System.Linq.Expressions;

namespace WolverineHttpWebAPI.Pagination;

public sealed class PageQueryDefaults
{
    public const int PageSize    = 20;
    public const int MaxPageSize = 50;
}

[DebuggerDisplay("Page = {Page}, PageSize = {PageSize}")]
public class PageQuery<TEntity>
{
    #region Fields: Page and PageSize
    private int _page;
    private int _pageSize;

    public int Page
    {
        get => _page;
        set => _page = value <= 0 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value <= 0 ? PageQueryDefaults.PageSize : value <= PageQueryDefaults.MaxPageSize ? value : PageQueryDefaults.MaxPageSize;
    }
    #endregion

    public Expression<Func<TEntity, bool>> FilterDefinition { get; set; }

    public Func<IQueryable<TEntity>, IQueryable<TEntity>>? SortDefinition { get; set; }

    public PageQuery(int? page = 1, int? pageSize = PageQueryDefaults.PageSize)
    {
        Page     = page     ?? 1;
        PageSize = pageSize ?? PageQueryDefaults.PageSize;

        FilterDefinition = _ => true;
    }

    public static PageQuery<TEntity> Create(int? page = 1, int? pageSize = PageQueryDefaults.PageSize)
    {
        return new PageQuery<TEntity>(page, pageSize);
    }
}

[DebuggerDisplay("Page = {Page}, PageSize = {PageSize}")]
public class PageQuery<TEntity, TProjection> : PageQuery<TEntity>
{
    public Expression<Func<TEntity, TProjection>>? ProjectionDefinition { get; set; }

    public PageQuery(int? page = 1, int? pageSize = PageQueryDefaults.PageSize) : base(page, pageSize)
    {

    }

    public new static PageQuery<TEntity, TProjection> Create(int? page = 1, int? pageSize = PageQueryDefaults.PageSize)
    {
        return new PageQuery<TEntity, TProjection>(page, pageSize);
    }
}