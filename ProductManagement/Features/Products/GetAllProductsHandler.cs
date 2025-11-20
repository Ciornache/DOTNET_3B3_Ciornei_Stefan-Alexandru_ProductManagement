using Microsoft.EntityFrameworkCore;
using ProductManagement.Persistence;

namespace ProductManagement.Features.Products;

/// <summary>
/// Handles retrieval of all products from the database.
/// </summary>
public class GetAllProductsHandler(ProductManagementContext context)
{
    /// <summary>
    /// Retrieves all products from the database.
    /// </summary>
    /// <param name="query">The query request for retrieving all products.</param>
    /// <returns>An IResult containing a list of all products.</returns>
    public async Task<IResult> Handle(GetAllProductsQuery query)
    {
        var products = await context.Products.ToListAsync();
        return Results.Ok(products);
    }
}