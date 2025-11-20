namespace ProductManagement.Features.Products;

/// <summary>
/// Command record for deleting a product from the database.
/// </summary>
/// <param name="Id">The unique identifier of the product to delete.</param>
public record DeleteProductCommand(Guid Id);
