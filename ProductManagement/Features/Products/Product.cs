namespace ProductManagement.Features.Products;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Brand { get; set; }
    public string SKU { get; set; }
    public decimal Price { get; set; }
    public ProductCategory Category { get; set; }
    public int StockQuantity { get; set; } = 0;
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; } = false;
    public DateTime ReleaseDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}