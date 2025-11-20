using MediatR;
using ProductManagement.Features.Products.DTOs;
using System.ComponentModel.DataAnnotations;
using ProductManagement.Validators.Attributes;

namespace ProductManagement.Features.Products;



public class CreateProductProfileCommand : IRequest<ProductProfileDto>
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 200 characters")]
    public required string Name { get; set; }
    
    [Required(ErrorMessage = "Brand is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Brand must be between 2 and 100 characters")]
    public required string Brand { get; set; }
    
    [Required(ErrorMessage = "SKU is required")]
    [ValidSKU]
    public required string SKU { get; set; }
    
    [Required(ErrorMessage = "Price is required")]
    [PriceRange(0.01f, 10000f)]
    public decimal Price { get; set; }
    
    [Required(ErrorMessage = "Category is required")]
    [ProductCategory(
        ProductCategory.Electronics, 
        ProductCategory.Clothing, 
        ProductCategory.Books, 
        ProductCategory.Home
    )]
    public ProductCategory Category { get; set; }
    
    [Range(0, 100000, ErrorMessage = "Stock quantity must be between 0 and 100,000")]
    public int StockQuantity { get; set; } = 1;
    
    [Url(ErrorMessage = "Please provide a valid URL")]
    public string? ImageUrl { get; set; }
    
    [Required(ErrorMessage = "Release date is required")]
    public DateTime ReleaseDate { get; set; }
}