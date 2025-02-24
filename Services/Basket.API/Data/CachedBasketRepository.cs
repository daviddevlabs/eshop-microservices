using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;

namespace Basket.API.Data;

public class CachedBasketRepository(IBasketRepository repository, HybridCache hybridCache, IDistributedCache cache)
    : IBasketRepository
{
    public async Task<ShoppingCart?> GetBasket(string userId, CancellationToken cancellationToken)
    {
        return await hybridCache.GetOrCreateAsync(userId, async _ =>
               await repository.GetBasket(userId, cancellationToken),
        cancellationToken: cancellationToken);
    }

    public async Task<ShoppingCart> StoreBasket(ShoppingCart basket, CancellationToken cancellationToken)
    {
        await repository.StoreBasket(basket, cancellationToken);
        await hybridCache.SetAsync(basket.UserId, basket, cancellationToken: cancellationToken);
        return basket;
    }

    public async Task<bool> DeleteBasket(string userId, CancellationToken cancellationToken)
    {
        await repository.DeleteBasket(userId, cancellationToken);
        await hybridCache.RemoveAsync(userId, cancellationToken);
        return true;
    }
}