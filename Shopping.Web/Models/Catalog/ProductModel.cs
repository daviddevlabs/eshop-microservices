namespace Shopping.Web.Models.Catalog;

public class ProductModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Price { get; set; }
    public string Category { get; set; } = default!;
    public List<string> ImageUrl { get; set; } = default!;
    public List<Specifications> Specifications { get; set; }
}

//wrapper classes
public record GetProductsResponse(PaginatedResult<ProductModel> Products);
public record GetProductByCategoryResponse(PaginatedResult<ProductModel> Products);
public record GetProductByIdResponse(ProductModel Product);