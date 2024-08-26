namespace Catalog.API.Products.GetProducts;

public record GetProductsQuery(int? PageNumber = 1, int? PageSize = 10) : IQuery<GetProductsResult>;
public record GetProductsResult(PaginatedResult<Product> Products);

internal class GetProductsQueryHandler
    (IDocumentSession session)
    : IQueryHandler<GetProductsQuery, GetProductsResult>
{
    public async Task<GetProductsResult> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        var totalCount = await session.Query<Product>().LongCountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling((decimal)totalCount / query.PageSize ?? 10);

        var products = await session.Query<Product>()
            .ToPagedListAsync(query.PageNumber ?? 1, query.PageSize ?? 10, cancellationToken);

        return new GetProductsResult(
            new PaginatedResult<Product>(
                query.PageNumber ?? 1,
                query.PageSize ?? 10,
                totalCount,
                totalPages,
                products));
    }
}
