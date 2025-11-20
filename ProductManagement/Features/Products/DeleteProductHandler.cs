using Microsoft.EntityFrameworkCore;
using ProductManagement.Persistence;

namespace ProductManagement.Features.Products;

/// <summary>
/// Handles deletion of products from the database.
/// </summary>
public class DeleteProductHandler(ProductManagementContext context)
{
    /// <summary>
    /// Deletes a product from the database by its unique identifier.
    /// </summary>
    /// <param name="command">The command containing the product ID to delete.</param>
    /// <returns>An IResult indicating success (NoContent) or failure (NotFound).</returns>
    public async Task<IResult> Handle(DeleteProductCommand command)
    {
        var product = await context.Products.FirstOrDefaultAsync(p => p.Id == command.Id);
        if (product == null)
        {
            return Results.NotFound($"Product with ID {command.Id} not found");
        }

        context.Products.Remove(product);
        await context.SaveChangesAsync();
        return Results.NoContent();
    }
}