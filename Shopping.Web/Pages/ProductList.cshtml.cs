namespace Shopping.Web.Pages;

public class ProductListModel
    (ICatalogService catalogService, IBasketService basketService, ILogger<ProductListModel> logger)
    : PageModel
{
    [BindProperty]
    public string Category { get; set; } = default!;
    public PaginatedResult<ProductModel> ProductList { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string category, int? pageNumber = 1, int? pageSize = 10)
    {
        if (category == null)
        {
            var response = await catalogService.GetProducts(pageNumber, pageSize);
            ProductList = response.Products;
        }
        else
        {
            var response = await catalogService.GetProductsByCategory(category, pageNumber, pageSize);
            ProductList = response.Products;
            Category = category;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAddToCartAsync(Guid productId)
    {
        logger.LogInformation("Add to cart button clicked");
        var productResponse = await catalogService.GetProduct(productId);

        var basket = await basketService.LoadUserBasket();

        basket.Items.Add(new ShoppingCartItemModel
        {
            ProductId = productId,
            ProductName = productResponse.Product.Title,
            Price = productResponse.Product.Price,
            Quantity = 1,
            Color = "Black"
        });

        await basketService.StoreBasket(new StoreBasketRequest(basket));
        return RedirectToPage("Cart");
    }
}