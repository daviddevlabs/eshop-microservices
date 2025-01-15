using BuildingBlocks.Security;
using Catalog.API;
using Microsoft.Extensions.Caching.Hybrid;

namespace Basket.API.Basket.GetBasket;

public record GetBasketQuery : IQuery<GetBasketResult>;
public record GetBasketResult(ShoppingCart? Cart);

public class GetBasketQueryHandler(
    ProductProtoService.ProductProtoServiceClient client, 
    HybridCache cache,
    IBasketRepository repository,
    IUserContextService userContext)
    : IQueryHandler<GetBasketQuery, GetBasketResult>
{
    public async Task<GetBasketResult> Handle(GetBasketQuery query, CancellationToken cancellationToken)
    {
        var basket = await repository.GetBasket(userContext.GetUserId(), cancellationToken);
        if (basket is null) return new GetBasketResult(null);
        
        foreach (var item in basket.Items)
        {
            var cachedProduct = await cache.GetOrCreateAsync($"products-{item.ProductId}", async _ => 
                await client.GetProductAsync(new GetProductRequest {
                    ProductId = item.ProductId.ToString()
                }, cancellationToken: cancellationToken), 
            tags: ["products"], 
            cancellationToken: cancellationToken);

            if (cachedProduct.Product is null) continue;
            item.QuantityAvailable = cachedProduct.Product.Quantity;
            item.Price = decimal.Parse(cachedProduct.Product.Price);
            item.IsAvailable = cachedProduct.Product.Quantity > 0;
        }
        return new GetBasketResult(basket);
    }
}
