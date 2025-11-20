using ProductManagement.Persistence;

namespace ProductManagement.Features.Products;

/// <summary>
/// Handles retrieval of a single product by its unique identifier.
/// </summary>
public class GetProductByIdHandler(ProductManagementContext dbContext)
{
    /// <summary>
    /// Retrieves a product by its unique identifier.
    /// </summary>
    /// <param name="query">The query containing the product ID to retrieve.</param>
    /// <returns>An IResult containing the product if found, or a NotFound result.</returns>
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