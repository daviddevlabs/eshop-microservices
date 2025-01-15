namespace Basket.API.Models;

public class ShoppingCartItem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; } 
    public decimal Price { get; set; }

    public bool IsAvailable { get; set; }
    public int QuantityAvailable { get; set; }
}
