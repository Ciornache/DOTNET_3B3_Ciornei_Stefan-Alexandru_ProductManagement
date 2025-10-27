using MediatR;
using ProductManagement.Features.Products.DTOs;

namespace ProductManagement.Features.Products;

public class CreateProductProfileCommand : IRequest<ProductProfileDto>
{
    public string Name { get; set; }
    public string Brand { get; set; }
    public string  SKU { get; set; }
    public decimal Price { get; set; }
    public ProductCategory Category { get; set; }
    public int StockQuantity { get; set; } = 1;
    public string? ImageUrl { get; set; }
    public DateTime ReleaseDate { get; set; }
}