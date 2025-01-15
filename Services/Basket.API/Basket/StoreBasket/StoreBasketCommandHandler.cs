using BuildingBlocks.Security;
using Catalog.API;
using Microsoft.Extensions.Caching.Hybrid;

namespace Basket.API.Basket.StoreBasket;

public record StoreBasketCommand(ShoppingCartItem Product, string Coupon) : ICommand<StoreBasketResult>;
public record StoreBasketResult(bool IsSuccess);

public class StoreBasketCommandValidator : AbstractValidator<StoreBasketCommand>
{
    public StoreBasketCommandValidator()
    {
        RuleFor(x => x.Product).NotNull()
            .When(x => string.IsNullOrEmpty(x.Coupon))
            .WithMessage("Either a Product or a Coupon must be provided.");
        RuleFor(x => x.Coupon).NotEmpty()
            .When(x => x?.Product == null)
            .WithMessage("A coupon must be provided if no Product is specified.");
    }
}

public class StoreBasketCommandHandler(
    ProductProtoService.ProductProtoServiceClient client, 
    HybridCache cache,
    IBasketRepository repository,
    IUserContextService userContext)
    : ICommandHandler<StoreBasketCommand, StoreBasketResult>
{
    public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
    {
        var basket = await repository.GetBasket(userContext.GetUserId(), cancellationToken);
        if (basket is null && command?.Product is null) return new StoreBasketResult(false);
        
        basket ??= new ShoppingCart 
        {
            UserId = userContext.GetUserId(),
            Items = []
        };
        
        if(command?.Product != null)
        {
            var cachedProduct = await cache.GetOrCreateAsync($"products-{command.Product.ProductId}", async _ => 
                await client.GetProductAsync(new GetProductRequest {
                    ProductId = command.Product.ProductId.ToString()
                }, cancellationToken: cancellationToken), 
            tags: ["products"], 
            cancellationToken: cancellationToken);
            
            if(cachedProduct == null) return new StoreBasketResult(false);

            var existingItem = basket.Items.FirstOrDefault(x => x.ProductId == command.Product.ProductId);
            if (existingItem == null)
            {
                basket.Items.Add(command.Product);
            }
            else
            {
                existingItem.Quantity = command.Product.Quantity;
            }
        }
        
        if(!string.IsNullOrEmpty(command?.Coupon)) basket.CouponCode = command.Coupon;
        await repository.StoreBasket(basket, cancellationToken);
        return new StoreBasketResult(true);
    }
}
