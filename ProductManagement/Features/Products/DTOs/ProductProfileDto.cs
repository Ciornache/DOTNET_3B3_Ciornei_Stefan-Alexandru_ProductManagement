namespace ProductManagement.Features.Products.DTOs;

public class ProductProfileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Brand { get; set; }
    public string SKU { get; set; }
    public decimal Price { get; set; }
    public string CategoryDisplayName { get; set; }
    public int StockQuantity { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; }
    public string ProductAge { get; set; }
    public string BrandInitials { get; set; }
    public string AvailabilityStatus { get; set; }
    public string FormattedPrice { get; set; }
}