namespace Basket.API.Exceptions;

public class BasketNotFoundException(string userId) : NotFoundException("Basket", userId);
