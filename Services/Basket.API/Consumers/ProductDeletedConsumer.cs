using BuildingBlocks.Messaging.Events.Product;
using MassTransit;
using Microsoft.Extensions.Caching.Hybrid;

namespace Basket.API.Consumers;

public class ProductDeletedConsumer(HybridCache cache) : IConsumer<ProductDeletedEvent>
{
    public async Task Consume(ConsumeContext<ProductDeletedEvent> context)
    {
        await cache.RemoveAsync($"products-{context.Message.Id}", context.CancellationToken);
    }
}