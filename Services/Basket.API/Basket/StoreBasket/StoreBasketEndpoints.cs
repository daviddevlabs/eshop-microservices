namespace Basket.API.Basket.StoreBasket;

public record StoreBasketRequest(CartProductDto Product, string Coupon);
public record StoreBasketResponse(bool IsSuccess);

public class StoreBasketEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/basket", async (StoreBasketRequest request, ISender sender) =>
        {
            var command = request.Adapt<StoreBasketCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<StoreBasketResponse>();

            return Results.Ok(response);
        })
        .WithName("CreateProductById")
        .Produces<StoreBasketResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Create Product By Id")
        .WithDescription("Create Product By Id")
        .RequireAuthorization();
    }
}
