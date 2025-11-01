using FluentValidation;
using ProductManagement.Persistence;

namespace ProductManagement.Features.Products;

public class UpdateProductHandler(ProductManagementContext dbContext, IValidator<UpdateProductCommand> validator)
{
    public async Task<IResult> Handle(UpdateProductCommand command)
    {
        var validationResult = await validator.ValidateAsync(command);
        
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        var existingProduct = await dbContext.Products.FindAsync(command.Id);
        if (existingProduct == null)
        {
            return Results.NotFound($"Product with ID {command.Id} not found");
        }
        
        // Update product properties
        existingProduct.Name = command.Name;
        existingProduct.Brand = command.Brand;
        existingProduct.SKU = command.SKU;
        existingProduct.Price = command.Price;
        existingProduct.Category = command.Category;
        existingProduct.StockQuantity = command.StockQuantity;
        existingProduct.ImageUrl = command.ImageUrl;
        existingProduct.IsAvailable = command.IsAvailable;
        existingProduct.ReleaseDate = command.ReleaseDate;
        existingProduct.UpdatedAt = DateTime.UtcNow;
        
        await dbContext.SaveChangesAsync();
        return Results.NoContent();
    }
}