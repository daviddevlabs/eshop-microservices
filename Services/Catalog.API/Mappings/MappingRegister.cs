namespace Catalog.API.Mappings;

public class MappingRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.Default.NameMatchingStrategy(NameMatchingStrategy.Flexible).IgnoreNullValues(true);
        config.NewConfig<Product, ProductModel>()
            .Map(dest => dest.ProductId, src => src.Id)
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Quantity, src => src.Quantity)
            .Map(dest => dest.Price, src => src.Price);
    }
}