using BuildingBlocks.Messaging.Events;
using BuildingBlocks.Security;
using Catalog.API;
using Discount.Grpc;
using MassTransit;
using Microsoft.Extensions.Caching.Hybrid;
using ShoppingCartItem = BuildingBlocks.Messaging.Events.ShoppingCartItem;

namespace Basket.API.Basket.CheckoutBasket;

public record CheckoutBasketCommand(BasketCheckoutDto BasketCheckoutDto) : ICommand<CheckoutBasketResult>;
public record CheckoutBasketResult(bool IsSuccess);

public class CheckoutBasketCommandValidator : AbstractValidator<CheckoutBasketCommand>
{
    public CheckoutBasketCommandValidator()
    {
        RuleFor(x => x.BasketCheckoutDto).NotNull().WithMessage("BasketCheckout can't be null");
    }
}

public class CheckoutBasketCommandHandler(
    ProductProtoService.ProductProtoServiceClient productProto,
    DiscountProtoService.DiscountProtoServiceClient discountProto,
    IBasketRepository repository,
    IPublishEndpoint publishEndpoint,
    HybridCache cache,
    IUserContextService userContext)
    : ICommandHandler<CheckoutBasketCommand, CheckoutBasketResult>
{
    public async Task<CheckoutBasketResult> Handle(CheckoutBasketCommand command, CancellationToken cancellationToken)
    {
        var basket = await repository.GetBasket(userContext.GetUserId(), cancellationToken);
        if(basket is null) return new CheckoutBasketResult(false);

        await DeductDiscount(basket, cancellationToken);
        if(basket.Items.Count == 0) return new CheckoutBasketResult(false);
        
        var eventMessage = command.BasketCheckoutDto.Adapt<BasketCheckoutEvent>();
        eventMessage.UserId = Guid.Parse(basket.UserId);
        eventMessage.UserName = "Bob";
        eventMessage.Items = basket.Items.Adapt<List<ShoppingCartItem>>();
        eventMessage.TotalPrice = eventMessage.Items.Sum(x => x.Price * x.Quantity);
        
        await publishEndpoint.Publish(eventMessage, cancellationToken);
        
        await repository.DeleteBasket(userContext.GetUserId(), cancellationToken);
        return new CheckoutBasketResult(true);
    }
    
    private async Task DeductDiscount(ShoppingCart cart, CancellationToken cancellationToken)
    {
        foreach (var item in cart.Items)
        {
            var cachedProduct = await cache.GetOrCreateAsync($"products-{item.ProductId}", async _ => 
                await productProto.GetProductAsync(new GetProductRequest {
                    ProductId = item.ProductId.ToString()
                }, cancellationToken: cancellationToken), 
            tags: ["products"], 
            cancellationToken: cancellationToken);
            
            if (cachedProduct.Product == null)
            {
                cart.ItemsToRemove.Add(item);
                continue;
            }
            
            item.Price = decimal.Parse(cachedProduct.Product.Price);
            if (string.IsNullOrEmpty(cart.CouponCode)) continue;
            
            var cachedCoupon = await cache.GetOrCreateAsync($"coupons-{item.ProductId}-{cart.CouponCode}", async _ => 
                await discountProto.GetDiscountAsync(new GetDiscountRequest { 
                    ProductId = item.ProductId.ToString(),
                    CouponCode = cart.CouponCode 
                }, cancellationToken: cancellationToken),
            tags: ["coupons"], 
            cancellationToken: cancellationToken);
            
            if(cachedCoupon.Amount != 0) item.Price -= cachedCoupon.Amount;
        }
        
        foreach (var item in cart.ItemsToRemove)
        {
            cart.Items.Remove(item);
        }
    }
}
