using Marten;

namespace WolverineHttpWebAPI.Pagination;

public static class Pagination
{
    public static async Task<PageResult<T>> PaginateAsync<T>(
        this IQueryable<T> queryable,
        PageQuery<T> query,
        CancellationToken cancellation = default)
    {
        (IQueryable<T> queryableItems, long totalCount, int pageCount) = await queryable.initQueryable(query, cancellation);

        if (totalCount == 0)
            return PageResult<T>.Empty;

        IEnumerable<T> items = await queryableItems.ToListAsync(cancellation);

        return new PageResult<T>(items, query.Page, query.PageSize, pageCount, totalCount);
    }

    public static async Task<PageResult<P>> PaginateAsync<T, P>(
        this IQueryable<T> queryable,
        PageQuery<T, P> query,
        CancellationToken cancellation = default)
    {
        if (query.ProjectionDefinition is null)
            throw new NullReferenceException("PageQuery.ProjectionDefinition can not be null.");

        (IQueryable<T> queryableItems, long totalCount, int pageCount) = await queryable.initQueryable(query, cancellation);

        if (totalCount == 0)
            return PageResult<P>.Empty;

        IEnumerable<P> items = await queryableItems
            .Select(query.ProjectionDefinition)
            .ToListAsync(cancellation);

        return new PageResult<P>(items, query.Page, query.PageSize, pageCount, totalCount);
    }

    private static async Task<(IQueryable<T> QueryableItems, long TotalCount, int PageCount)> initQueryable<T>(
        this IQueryable<T> queryable,
        PageQuery<T> query,
        CancellationToken cancellation)
    {
        if (query.FilterDefinition is null)
            throw new NullReferenceException("PageQuery.FilterDefinition can not be null.");

        long totalCount = await queryable.CountAsync(query.FilterDefinition, cancellation);

        if (totalCount == 0)
            return (Enumerable.Empty<T>().AsQueryable(), 0, 0);

        int pageCount = (int)Math.Ceiling((decimal)totalCount / query.PageSize);

        if (query.Page > pageCount)
            query.Page = pageCount;

        int skip = (query.Page - 1) * query.PageSize;

        IQueryable<T> queryableItems = queryable.Where(query.FilterDefinition);

        if (query.SortDefinition is not null)
            queryableItems = query.SortDefinition(queryableItems);

        queryableItems = queryableItems
            .Skip(skip)
            .Take(query.PageSize);

        return (queryableItems, totalCount, pageCount);
    }
}