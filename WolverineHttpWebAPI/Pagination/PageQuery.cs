using System.Diagnostics;
using System.Linq.Expressions;

namespace WolverineHttpWebAPI.Pagination;

public sealed class PageQueryDefaults
{
    public const int PageSizeDefault = 20;
    public const int PageSizeMax     = 50;
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
        set => _pageSize = value <= 0 ? PageQueryDefaults.PageSizeDefault : value <= PageQueryDefaults.PageSizeMax ? value : PageQueryDefaults.PageSizeMax;
    }
    #endregion

    public Expression<Func<TEntity, bool>>? FilterDefinition { get; set; }

    public Func<IQueryable<TEntity>, IQueryable<TEntity>>? SortDefinition { get; set; }

    public PageQuery(int? page = 1, int? pageSize = PageQueryDefaults.PageSizeDefault)
    {
        Page     = page     ?? 1;
        PageSize = pageSize ?? PageQueryDefaults.PageSizeDefault;
    }

    public static PageQuery<TEntity> Create(int? page = 1, int? pageSize = PageQueryDefaults.PageSizeDefault)
    {
        return new PageQuery<TEntity>(page, pageSize);
    }
}

[DebuggerDisplay("Page = {Page}, PageSize = {PageSize}")]
public class PageQuery<TEntity, TProjection> : PageQuery<TEntity>
{
    public Expression<Func<TEntity, TProjection>>? ProjectionDefinition { get; set; }

    public PageQuery(int? page = 1, int? pageSize = PageQueryDefaults.PageSizeDefault) : base(page, pageSize)
    {

    }

    public new static PageQuery<TEntity, TProjection> Create(int? page = 1, int? pageSize = PageQueryDefaults.PageSizeDefault)
    {
        return new PageQuery<TEntity, TProjection>(page, pageSize);
    }
}