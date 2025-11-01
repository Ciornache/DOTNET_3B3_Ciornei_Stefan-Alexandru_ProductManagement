using Microsoft.EntityFrameworkCore;
using ProductManagement.Persistence;

namespace ProductManagement.Features.Products;

public class GetAllProductsHandler(ProductManagementContext context)
{
    public async Task<IResult> Handle(GetAllProductsQuery query)
    {
        var products = await context.Products.ToListAsync();
        return Results.Ok(products);
    }
}