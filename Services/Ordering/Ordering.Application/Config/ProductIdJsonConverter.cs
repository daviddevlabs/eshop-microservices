using Newtonsoft.Json;

namespace Ordering.Application.Config;

public class ProductIdJsonConverter : JsonConverter<ProductId>
{
    public override void WriteJson(JsonWriter writer, ProductId? value, JsonSerializer serializer)
    {
        writer.WriteValue(value?.Value);
    }

    public override ProductId ReadJson(JsonReader reader, Type objectType, ProductId? existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        var value = Guid.Parse(reader.Value?.ToString()!);
        return ProductId.Of(value);
    }
}