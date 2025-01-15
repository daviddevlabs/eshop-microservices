using System.Globalization;
using BuildingBlocks.Messaging.Events.Product;
using Catalog.API;
using MassTransit;
using Microsoft.Extensions.Caching.Hybrid;

namespace Basket.API.Consumers;

public class ProductUpdatedConsumer(HybridCache cache) : IConsumer<ProductUpdatedEvent>
{
    public async Task Consume(ConsumeContext<ProductUpdatedEvent> context)
    {
        await cache.SetAsync($"products-{context.Message.Id}", new GetProductResponse { Product = new ProductModel
            {
                ProductId = context.Message.Id.ToString(),
                Quantity = context.Message.Quantity,
                Price = context.Message.Price.ToString(CultureInfo.InvariantCulture)
            }}, 
            tags: ["products"], 
            cancellationToken: context.CancellationToken);
    }
}