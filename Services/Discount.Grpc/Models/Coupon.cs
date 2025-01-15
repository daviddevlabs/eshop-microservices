namespace Discount.Grpc.Models;

public class Coupon
{
    public int Id { get; init; }
    public Guid ProductId { get; init; }
    public string CouponCode { get; init; } = null!;
    public string Description { get; init; } = null!;
    public int Amount { get; init; }
}
