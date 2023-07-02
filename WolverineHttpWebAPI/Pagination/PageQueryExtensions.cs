using System.Linq.Expressions;

namespace WolverineHttpWebAPI.Pagination;

public static class PageQueryExtensions
{
    // Filter for PageQuery<T>
    public static PageQuery<T> Filter<T>(this PageQuery<T> pageQuery, Expression<Func<T, bool>> filterExp)
    {
        pageQuery.FilterDefinition = filterExp;

        return pageQuery;
    }

    // Sort for PageQuery<T>
    public static PageQuery<T> Sort<T, TKey>(this PageQuery<T> pageQuery, Expression<Func<T, TKey>> sortFunc)
    {
        pageQuery.SortDefinition = items => items.OrderBy(sortFunc);

        return pageQuery;
    }

    // Filter for PageQuery<T, P>
    public static PageQuery<T, P> Filter<T, P>(this PageQuery<T, P> pageQuery, Expression<Func<T, bool>> filterExp)
    {
        pageQuery.FilterDefinition = filterExp;

        return pageQuery;
    }

    // Sort for PageQuery<T, P>
    public static PageQuery<T, P> Sort<T, P, TKey>(this PageQuery<T, P> pageQuery, Expression<Func<T, TKey>> sortFunc)
    {
        pageQuery.SortDefinition = items => items.OrderBy(sortFunc);

        return pageQuery;
    }

    // Project for PageQuery<T, P>
    public static PageQuery<T, P> Project<T, P>(this PageQuery<T, P> pageQuery, Expression<Func<T, P>> projectionExp)
    {
        pageQuery.ProjectionDefinition = projectionExp;

        return pageQuery;
    }
}
