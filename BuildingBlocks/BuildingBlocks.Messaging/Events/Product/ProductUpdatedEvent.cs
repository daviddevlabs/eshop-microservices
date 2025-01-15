namespace BuildingBlocks.Messaging.Events.Product;

public class ProductUpdatedEvent
{
    public Guid Id { get; init; }
    public string Title { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}