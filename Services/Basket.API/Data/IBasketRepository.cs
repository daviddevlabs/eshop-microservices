namespace Basket.API.Data;

public interface IBasketRepository
{
    Task<ShoppingCart?> GetBasket(string userId, CancellationToken cancellationToken);
    Task<ShoppingCart> StoreBasket(ShoppingCart basket, CancellationToken cancellationToken);
    Task<bool> DeleteBasket(string userId, CancellationToken cancellationToken);
}
