namespace ProductManagement.Features.Products.DTOs;

/// <summary>
/// Data Transfer Object representing a product profile with computed display properties.
/// </summary>
public class ProductProfileDto
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
    /// Gets or sets the formatted category display name (e.g., "Electronics & Technology").
    /// </summary>
    public required string CategoryDisplayName { get; set; }
    
    /// <summary>
    /// Gets or sets the quantity of items in stock.
    /// </summary>
    public int StockQuantity { get; set; }
    
    /// <summary>
    /// Gets or sets the URL to the product image. May be null based on category filtering.
    /// </summary>
    public string? ImageUrl { get; set; }
    
    /// <summary>
    /// Gets or sets whether the product is available for purchase.
    /// </summary>
    public bool IsAvailable { get; set; }
    
    /// <summary>
    /// Gets or sets the human-readable product age description (e.g., "6 months old", "Classic").
    /// </summary>
    public required string ProductAge { get; set; }
    
    /// <summary>
    /// Gets or sets the brand initials extracted from the brand name.
    /// </summary>
    public required string BrandInitials { get; set; }
    
    /// <summary>
    /// Gets or sets the availability status description (e.g., "In Stock", "Limited Stock").
    /// </summary>
    public required string AvailabilityStatus { get; set; }
    
    /// <summary>
    /// Gets or sets the formatted price string with currency symbol and discounts applied.
    /// </summary>
    public required string FormattedPrice { get; set; }
}