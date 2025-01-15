namespace Catalog.API.Models;

public class Specification
{
    public int Order { get; set; }
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
}
