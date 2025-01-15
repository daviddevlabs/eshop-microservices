using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Basket.API.Data;

public class CachedBasketRepository(IBasketRepository repository, IDistributedCache cache) : IBasketRepository
{
    public async Task<ShoppingCart?> GetBasket(string userId, CancellationToken cancellationToken)
    {
        // var cachedBasket = await cache.GetStringAsync(userId, cancellationToken);
        // if (!string.IsNullOrEmpty(cachedBasket)) 
        //     return JsonSerializer.Deserialize<ShoppingCart>(cachedBasket)!;

        var basket = await repository.GetBasket(userId, cancellationToken);
        //await cache.SetStringAsync(userId, JsonSerializer.Serialize(basket), cancellationToken);
        return basket;
    }

    public async Task<ShoppingCart> StoreBasket(ShoppingCart basket, CancellationToken cancellationToken)
    {
        await repository.StoreBasket(basket, cancellationToken);
        // await cache.SetStringAsync(basket.UserId, JsonSerializer.Serialize(basket), cancellationToken);
        return basket;
    }

    public async Task<bool> DeleteBasket(string userId, CancellationToken cancellationToken)
    {
        await repository.DeleteBasket(userId, cancellationToken);
        // await cache.RemoveAsync(userId, cancellationToken);
        return true;
    }
}