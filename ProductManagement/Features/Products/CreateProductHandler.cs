using FluentValidation;
using ProductManagement.Persistence;

namespace ProductManagement.Features.Products;

public class CreateProductHandler(ProductManagementContext context, ILogger<CreateProductHandler> logger, IValidator<CreateProductCommand> validator)
{
    public async Task<IResult> Handle(CreateProductCommand command)
    {
        logger.LogInformation("Creating new product with Name: {Name}, SKU: {SKU}, and Brand: {Brand}", command.Name, command.SKU, command.Brand);

        var validationResult = await validator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
             throw new ValidationException(validationResult.Errors);
        }
        
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Brand = command.Brand,
            SKU = command.SKU,
            Price = command.Price,
            Category = command.Category,
            StockQuantity = command.StockQuantity,
            ImageUrl = command.ImageUrl,
            IsAvailable = command.IsAvailable,
            ReleaseDate = command.ReleaseDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        context.Products.Add(product);
        await context.SaveChangesAsync();
        logger.LogInformation("Product created successfully with ID: {ProductId}", product.Id);

        return Results.Created($"/products/{product.Id}", product);
    }
}