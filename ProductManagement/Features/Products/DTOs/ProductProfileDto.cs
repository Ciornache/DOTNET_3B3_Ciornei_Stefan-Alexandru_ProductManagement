namespace ProductManagement.Features.Products.DTOs;

public class ProductProfileDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Brand { get; set; }
    public required string SKU { get; set; }
    public decimal Price { get; set; }
    public required string CategoryDisplayName { get; set; }
    public int StockQuantity { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; }
    public required string ProductAge { get; set; }
    public required string BrandInitials { get; set; }
    public required string AvailabilityStatus { get; set; }
    public required string FormattedPrice { get; set; }
}