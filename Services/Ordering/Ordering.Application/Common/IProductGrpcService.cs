namespace Ordering.Application.Common;

public interface IProductGrpcService
{
    Task<List<Product>> GetProductsByIdsAsync(List<Guid> productsIds);
}