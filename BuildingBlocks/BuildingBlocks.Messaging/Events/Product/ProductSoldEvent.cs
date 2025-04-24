namespace BuildingBlocks.Messaging.Events.Product;

public class ProductSoldEvent
{
    public List<ProductSold> ProductsSold { get; set; } = [];
}

public class ProductSold
{
    public Guid Id { get; init; }
    public int Quantity { get; set; }
}