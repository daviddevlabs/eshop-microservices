namespace Catalog.API.Products.GetProductByCategory;

public record GetProductByCategoryQuery(string Category, int PageNumber, int PageSize) : IQuery<GetProductByCategoryResult>;
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

        var totalPages = (int)Math.Ceiling((decimal)totalCount / query.PageSize);

        var products = await session.Query<Product>()
            .Where(p => p.Category.Contains(query.Category))
            .ToPagedListAsync(query.PageNumber, query.PageSize, cancellationToken);


        return new GetProductByCategoryResult(
            new PaginatedResult<Product>(
                query.PageNumber,
                query.PageSize,
                totalCount,
                totalPages,
                products));
    }
}