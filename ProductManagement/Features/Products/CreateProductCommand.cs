namespace ProductManagement.Features.Products;

/// <summary>
/// Command record for creating a new product with basic properties.
/// </summary>
/// <param name="Name">The product name.</param>
/// <param name="Brand">The brand name.</param>
/// <param name="SKU">The Stock Keeping Unit identifier.</param>
/// <param name="Price">The product price.</param>
/// <param name="Category">The product category.</param>
/// <param name="StockQuantity">The quantity in stock.</param>
/// <param name="ImageUrl">The product image URL (optional).</param>
/// <param name="IsAvailable">Whether the product is available for purchase.</param>
/// <param name="ReleaseDate">The product release date.</param>
public record CreateProductCommand(
    string Name, 
    string Brand, 
    string SKU, 
    decimal Price, 
    ProductCategory Category, 
    int StockQuantity, 
    string? ImageUrl, 
    bool IsAvailable, 
    DateTime ReleaseDate
);
