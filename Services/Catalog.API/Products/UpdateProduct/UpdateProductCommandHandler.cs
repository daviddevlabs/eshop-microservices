namespace Catalog.API.Products.UpdateProduct;

public record UpdateProductCommand(Guid Id, string Title, string Description, decimal Price, string Category, List<string> ImageUrl, List<Specification> Specifications)
    : ICommand<UpdateProductResult>;
public record UpdateProductResult(bool IsSuccess);

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty().WithMessage("Product ID is required");

        RuleFor(command => command.Title)
            .NotEmpty().WithMessage("Title is required")
            .Length(2, 150).WithMessage("Title must be between 2 and 150 characters");

        RuleFor(command => command.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");
    }
}

internal class UpdateProductCommandHandler
    (IDocumentSession session)
    : ICommandHandler<UpdateProductCommand, UpdateProductResult>
{
    public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var product = await session.LoadAsync<Product>(command.Id, cancellationToken);

        if (product is null)
        {
            throw new ProductNotFoundException(command.Id);
        };

        product.Title = command.Title;
        product.Description = command.Description;
        product.Category = command.Category;
        product.Price = command.Price;
        product.ImageUrl = command.ImageUrl;
        product.Specifications = command.Specifications;

        session.Update(product);
        await session.SaveChangesAsync(cancellationToken);

        return new UpdateProductResult(true);
    }
}