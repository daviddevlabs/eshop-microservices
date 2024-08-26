namespace Catalog.API.Models;

public class Specification
{
    public int Order { get; set; }
    public string Key { get; set; } = default!;
    public string Value { get; set; } = default!;
}
