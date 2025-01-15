namespace Basket.API.Data;

public class BasketRepository(IDocumentSession session) : IBasketRepository
{
    public async Task<ShoppingCart?> GetBasket(string userId, CancellationToken cancellationToken)
    {
        return await session.LoadAsync<ShoppingCart>(userId, cancellationToken);
    }

    public async Task<ShoppingCart> StoreBasket(ShoppingCart basket, CancellationToken cancellationToken)
    {
        session.Store(basket);
        await session.SaveChangesAsync(cancellationToken);
        return basket;
    }

    public async Task<bool> DeleteBasket(string userId, CancellationToken cancellationToken)
    {
        session.Delete<ShoppingCart>(userId);
        await session.SaveChangesAsync(cancellationToken);
        return true;
    }
}
