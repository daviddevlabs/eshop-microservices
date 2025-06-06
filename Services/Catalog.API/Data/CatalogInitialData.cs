﻿using Marten.Schema;

namespace Catalog.API.Data;

public class CatalogInitialData : IInitialData
{
    public async Task Populate(IDocumentStore store, CancellationToken cancellationToken)
    {
        await using var session = store.LightweightSession();
        if (await session.Query<Product>().AnyAsync(cancellationToken)) return;

        session.Store(GetPreconfiguredProducts());
        await session.SaveChangesAsync(cancellationToken);
    }

    private static IEnumerable<Product> GetPreconfiguredProducts() => 
    [
        new()
        {
            Id = new Guid("5334c996-8457-4cf0-815c-ed2b77c4ff61"),
            Title = "IPhone X",
            Description =
                "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
            ImageUrl = new List<string> { "product-1.png" },
            Price = 950.00M,
            Category = "SmartPhone",
            Specifications = new List<Specification>() { }
        },
        new()
        {
            Id = new Guid("c67d6323-e8b1-4bdf-9a75-b0d0d2e7e914"),
            Title = "Samsung 10",
            Description =
                "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
            ImageUrl = new List<string> { "product-2.png" },
            Price = 840.00M,
            Category = "SmartPhone",
            Specifications = new List<Specification>() { }
        },
        new()
        {
            Id = new Guid("4f136e9f-ff8c-4c1f-9a33-d12f689bdab8"),
            Title = "Huawei Plus",
            Description =
                "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
            ImageUrl = new List<string> { "product-3.png" },
            Price = 650.00M,
            Category = "WhiteAppliances",
            Specifications = new List<Specification>() { }
        },
        new()
        {
            Id = new Guid("6ec1297b-ec0a-4aa1-be25-6726e3b51a27"),
            Title = "Xiaomi Mi 9",
            Description =
                "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
            ImageUrl = new List<string> { "product-4.png" },
            Price = 470.00M,
            Category = "WhiteAppliances",
            Specifications = new List<Specification>() { }
        },
        new()
        {
            Id = new Guid("b786103d-c621-4f5a-b498-23452610f88c"),
            Title = "HTC U11+ Plus",
            Description =
                "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
            ImageUrl = new List<string> { "product-5.png" },
            Price = 380.00M,
            Category = "SmartPhone",
            Specifications = new List<Specification>() { }
        },
        new()
        {
            Id = new Guid("c4bbc4a2-4555-45d8-97cc-2a99b2167bff"),
            Title = "LG G7 ThinQ",
            Description =
                "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
            ImageUrl = new List<string> { "product-6.png" },
            Price = 240.00M,
            Category = "Home itchen",
            Specifications = new List<Specification>() { }
        },
        new()
        {
            Id = new Guid("93170c85-7795-489c-8e8f-7dcf3b4f4188"),
            Title = "Panasonic Lumix",
            Description =
                "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
            ImageUrl = new List<string> { "product-7.png" },
            Price = 240.00M,
            Category = "Camera",
            Specifications = new List<Specification>() { }
        }
    ];
}
