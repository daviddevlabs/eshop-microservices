namespace Catalog.API.Products.GetProducts;

public record GetProductsQuery(int PageNumber, int PageSize) : IQuery<GetProductsResult>;
public record GetProductsResult(PaginatedResult<Product> Products);

internal class GetProductsQueryHandler
    (IDocumentSession session)
    : IQueryHandler<GetProductsQuery, GetProductsResult>
{
    public async Task<GetProductsResult> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        var totalCount = await session.Query<Product>().LongCountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling((decimal)totalCount / query.PageSize);

        var products = await session.Query<Product>()
            .ToPagedListAsync(query.PageNumber, query.PageSize, cancellationToken);

        return new GetProductsResult(
            new PaginatedResult<Product>(
                query.PageNumber,
                query.PageSize,
                totalCount,
                totalPages,
                products));
    }
}
