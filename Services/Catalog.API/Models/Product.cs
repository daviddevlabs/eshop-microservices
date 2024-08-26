namespace Catalog.API.Models;

public class Product
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Price { get; set; } = default!;
    public string Category { get; set; } = default!;
    public List<string> ImageUrl { get; set; } = default!;
    public List<Specification> Specifications { get; set; } = new();

    public static Product Create(string title, string description, decimal price, string category)
    {
        return new Product
        {
            Title = title,
            Description = description,
            Price = price,
            Category = category
        };
    }

    public void AddSpecification(string key, string value)
    {
        var order = Specifications.Count != 0 ? Specifications.Max(s => s.Order) + 1 : 1;
        Specifications.Add(new Specification { Key = key, Value = value, Order = order });
    }

    public void RemoveSpecification(string key)
    {
        var specification = Specifications.Find(s => s.Key == key);
        if (specification != null) Specifications.Remove(specification);
    }

    public void ReorderSpecification(string key, int newOrder)
    {
        var specification = Specifications.Find(s => s.Key == key);

        if (specification != null)
        {
            Specifications.Remove(specification);
            specification.Order = newOrder;
            Specifications.Add(specification);
            Specifications = Specifications.OrderBy(s => s.Order).ToList();
        }
    }
}
