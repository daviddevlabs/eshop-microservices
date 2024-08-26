namespace Catalog.API.Products.GetProductByCategory;

public record GetProductByCategoryQuery(string Category, int? PageNumber = 1, int? PageSize = 10) : IQuery<GetProductByCategoryResult>;
public record GetProductByCategoryResult(PaginatedResult<Product> Products);

internal class GetProductByCategoryQueryHandler
    (IDocumentSession session)
    : IQueryHandler<GetProductByCategoryQuery, GetProductByCategoryResult>
{
    public async Task<GetProductByCategoryResult> Handle(GetProductByCategoryQuery query, CancellationToken cancellationToken)
    {
        var totalCount = await session.Query<Product>()
            .Where(p => p.Category.Contains(query.Category))
            .LongCountAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling((decimal)totalCount / query.PageSize ?? 10);

        var products = await session.Query<Product>()
            .Where(p => p.Category.Contains(query.Category))
            .ToPagedListAsync(query.PageNumber ?? 1, query.PageSize ?? 10, cancellationToken);


        return new GetProductByCategoryResult(
            new PaginatedResult<Product>(
                query.PageNumber ?? 1,
                query.PageSize ?? 10,
                totalCount,
                totalPages,
                products));
    }
}