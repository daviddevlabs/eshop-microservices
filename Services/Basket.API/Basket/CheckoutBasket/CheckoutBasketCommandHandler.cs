using BuildingBlocks.Messaging.Discount;
using BuildingBlocks.Messaging.Events;
using BuildingBlocks.Messaging.Product;
using BuildingBlocks.Security;
using MassTransit;
using Microsoft.Extensions.Caching.Hybrid;

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
        if (basket is null) return new CheckoutBasketResult(false);

        await DeductCartItems(basket, cancellationToken);
        if (basket.Items.Count == 0) return new CheckoutBasketResult(false);

        var eventMessage = command.BasketCheckoutDto.Adapt<BasketCheckoutEvent>();
        eventMessage.UserId = Guid.Parse(basket.UserId);
        eventMessage.UserName = userContext.GetUserName();
        eventMessage.Products = basket.Items.Adapt<List<ShoppingCartProducts>>();
        eventMessage.TotalPrice = eventMessage.Products.Sum(x => x.Price * x.Quantity);

        await publishEndpoint.Publish(eventMessage, cancellationToken);
        await repository.DeleteBasket(userContext.GetUserId(), cancellationToken);

        return new CheckoutBasketResult(true);
    }

    private async Task DeductCartItems(ShoppingCart cart, CancellationToken cancellationToken)
    {
        var itemsToRemove = new List<ShoppingCartItem>();

        foreach (var item in cart.Items)
        {
            var cachedProduct = await GetProductFromCacheAsync(item.ProductId, cancellationToken);
            if (cachedProduct.Product == null || cachedProduct.Product.Quantity < item.Quantity)
            {
                itemsToRemove.Add(item);
                continue;
            }

            cachedProduct.Product.Quantity -= item.Quantity;
            await cache.SetAsync($"products-{cachedProduct.Product.ProductId}", cachedProduct, cancellationToken: cancellationToken);

            if (decimal.TryParse(cachedProduct.Product.Price, out var price)) item.Price = price;
            if (string.IsNullOrEmpty(cart.CouponCode)) continue;

            var cachedCoupon = await GetDiscountFromCacheAsync(item.ProductId, cart.CouponCode, cancellationToken);
            if (cachedCoupon != null) item.Price -= cachedCoupon.Amount;
        }

        foreach (var item in itemsToRemove) cart.Items.Remove(item);
    }

    private async Task<GetProductResponse> GetProductFromCacheAsync(Guid productId, CancellationToken token)
    {
        return await cache.GetOrCreateAsync($"products-{productId}",
            async _ => await productProto.GetProductAsync(new GetProductRequest
            {
                ProductId = productId.ToString()
            }, cancellationToken: token),
        tags: ["products"],
        cancellationToken: token);
    }

    private async Task<CouponModel?> GetDiscountFromCacheAsync(Guid productId, string couponCode, CancellationToken token)
    {
        return await cache.GetOrCreateAsync($"coupons-{productId}-{couponCode}",
            async _ => await discountProto.GetDiscountAsync(new GetDiscountRequest
            {
                ProductId = productId.ToString(),
                CouponCode = couponCode
            }, cancellationToken: token),
        tags: ["coupons"],
        cancellationToken: token);
    }
}
