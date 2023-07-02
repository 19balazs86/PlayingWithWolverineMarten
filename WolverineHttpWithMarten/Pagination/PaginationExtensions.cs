using Marten;

namespace WolverineHttpWithMarten.Pagination;

public static class PaginationExtensions
{
    public static async Task<PageResult<T>> PaginateAsync<T>(
        this IQueryable<T> queryable,
        PageQuery<T> query,
        CancellationToken cancellation = default)
    {
        return await queryable.innerPaginateAsync(query, applyProjection, cancellation);

        static IQueryable<T> applyProjection(IQueryable<T> tQueryable) => tQueryable; // Local function. No projection
    }

    public static async Task<PageResult<P>> PaginateAsync<T, P>(
        this IQueryable<T> queryable,
        PageQuery<T, P> query,
        CancellationToken cancellation = default)
    {
        if (query.ProjectionDefinition is null)
            throw new NullReferenceException("PageQuery.ProjectionDefinition can not be null.");

        return await queryable.innerPaginateAsync(query, applyProjection, cancellation);

        IQueryable<P> applyProjection(IQueryable<T> tQueryable) // Local function
        {
            return tQueryable.Select(query.ProjectionDefinition);
        }
    }

    private static async Task<PageResult<P>> innerPaginateAsync<T, P>(
        this IQueryable<T> queryable,
        PageQuery<T> query,
        Func<IQueryable<T>, IQueryable<P>> applyProjection,
        CancellationToken cancellation)
    {
        // There is a Marten.Pagination.PagedList<> class for pagination, but I found the following better

        if (query.FilterDefinition is not null)
        {
            queryable = queryable.Where(query.FilterDefinition);
        }

        long totalCount = await queryable.CountAsync(cancellation); // CountAsync(query.FilterDefinition);

        if (totalCount == 0)
        {
            return PageResult<P>.Empty;
        }

        int pageCount = (int)Math.Ceiling((decimal)totalCount / query.PageSize);

        if (query.Page > pageCount)
        {
            query.Page = pageCount;
        }

        int skip = (query.Page - 1) * query.PageSize;

        if (query.SortDefinition is not null)
        {
            queryable = query.SortDefinition(queryable);
        }

        queryable = queryable
            .Skip(skip)
            .Take(query.PageSize);

        IQueryable<P> projectionQueryable = applyProjection(queryable);

        IEnumerable<P> items = await projectionQueryable.ToListAsync(cancellation);

        return new PageResult<P>(items, query.Page, query.PageSize, pageCount, totalCount);
    }
}