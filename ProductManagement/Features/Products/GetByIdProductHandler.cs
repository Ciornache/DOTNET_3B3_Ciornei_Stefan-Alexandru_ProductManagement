using ProductManagement.Persistence;

namespace ProductManagement.Features.Products;

public class GetProductByIdHandler(ProductManagementContext dbContext)
{
    public async Task<IResult> Handle(GetProductByIdQuery query)
    {
        var product = await dbContext.Products.FindAsync(query.Id);
        if (product == null)
        {
            return Results.NotFound($"Product with ID {query.Id} not found");
        }
        return Results.Ok(product);
    }
}