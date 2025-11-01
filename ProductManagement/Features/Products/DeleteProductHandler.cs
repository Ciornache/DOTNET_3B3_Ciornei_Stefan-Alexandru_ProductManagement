using Microsoft.EntityFrameworkCore;
using ProductManagement.Persistence;

namespace ProductManagement.Features.Products;

public class DeleteProductHandler(ProductManagementContext context)
{
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