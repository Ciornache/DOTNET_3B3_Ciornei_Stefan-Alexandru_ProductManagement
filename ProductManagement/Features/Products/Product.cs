namespace ProductManagement.Features.Products;

/// <summary>
/// Represents a product entity in the product management system.
/// </summary>
public class Product
{
    /// <summary>
    /// Gets or sets the unique identifier for the product.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets or sets the product name.
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Gets or sets the brand name of the product.
    /// </summary>
    public required string Brand { get; set; }
    
    /// <summary>
    /// Gets or sets the Stock Keeping Unit (SKU) identifier.
    /// </summary>
    public required string SKU { get; set; }
    
    /// <summary>
    /// Gets or sets the price of the product.
    /// </summary>
    public decimal Price { get; set; }
    
    /// <summary>
    /// Gets or sets the category of the product.
    /// </summary>
    public ProductCategory Category { get; set; }
    
    /// <summary>
    /// Gets or sets the quantity of items in stock. Default is 0.
    /// </summary>
    public int StockQuantity { get; set; } = 0;
    
    /// <summary>
    /// Gets or sets the URL to the product image. Can be null.
    /// </summary>
    public string? ImageUrl { get; set; }
    
    /// <summary>
    /// Gets or sets whether the product is available for purchase. Default is false.
    /// </summary>
    public bool IsAvailable { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the date when the product was released.
    /// </summary>
    public DateTime ReleaseDate { get; set; }
    
    /// <summary>
    /// Gets or sets the timestamp when the product was created in the system.
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the timestamp when the product was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}