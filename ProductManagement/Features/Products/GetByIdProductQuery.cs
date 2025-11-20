namespace ProductManagement.Features.Products;

/// <summary>
/// Query record for retrieving a single product by its unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the product to retrieve.</param>
public record GetProductByIdQuery(Guid Id);
