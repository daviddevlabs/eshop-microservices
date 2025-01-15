using System.Globalization;
using Grpc.Core;

namespace Catalog.API.Services;

public class ProductService(IDocumentSession session, ILogger<ProductService> logger) 
    : ProductProtoService.ProductProtoServiceBase
{
    public override async Task<GetProductResponse?> GetProduct(GetProductRequest request, ServerCallContext context)
    {
        var product = await session.LoadAsync<Product>(Guid.Parse(request.ProductId));
        if (product is null) return new GetProductResponse { Product = null };
        
        logger.LogInformation("Product is retrieved: {@product}", product);
        return new GetProductResponse
        {
            Product = new ProductModel
            {
                ProductId = product.Id.ToString(),
                Quantity = product.Quantity,
                Price = product.Price.ToString(CultureInfo.InvariantCulture)
            }
        };
    }
}