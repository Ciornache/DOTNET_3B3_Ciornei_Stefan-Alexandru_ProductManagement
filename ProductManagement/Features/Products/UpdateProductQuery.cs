namespace ProductManagement.Features.Products;

public record UpdateProductCommand(
    Guid Id, 
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
