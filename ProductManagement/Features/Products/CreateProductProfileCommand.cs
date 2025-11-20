using MediatR;
using ProductManagement.Features.Products.DTOs;
using System.ComponentModel.DataAnnotations;
using ProductManagement.Validators.Attributes;

namespace ProductManagement.Features.Products;

/// <summary>
/// Command for creating a new product profile with comprehensive validation.
/// </summary>
public class CreateProductProfileCommand : IRequest<ProductProfileDto>
{
    /// <summary>
    /// Gets or sets the product name. Must be between 1 and 200 characters.
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 200 characters")]
    public required string Name { get; set; }
    
    /// <summary>
    /// Gets or sets the brand name. Must be between 2 and 100 characters.
    /// </summary>
    [Required(ErrorMessage = "Brand is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Brand must be between 2 and 100 characters")]
    public required string Brand { get; set; }
    
    /// <summary>
    /// Gets or sets the Stock Keeping Unit (SKU). Must follow the valid SKU format.
    /// </summary>
    [Required(ErrorMessage = "SKU is required")]
    [ValidSKU]
    public required string SKU { get; set; }
    
    /// <summary>
    /// Gets or sets the product price. Must be between 0.01 and 10,000.
    /// </summary>
    [Required(ErrorMessage = "Price is required")]
    [PriceRange(0.01f, 10000f)]
    public decimal Price { get; set; }
    
    /// <summary>
    /// Gets or sets the product category. Must be one of the allowed categories.
    /// </summary>
    [Required(ErrorMessage = "Category is required")]
    [ProductCategory(
        ProductCategory.Electronics, 
        ProductCategory.Clothing, 
        ProductCategory.Books, 
        ProductCategory.Home
    )]
    public ProductCategory Category { get; set; }
    
    /// <summary>
    /// Gets or sets the stock quantity. Must be between 0 and 100,000. Default is 1.
    /// </summary>
    [Range(0, 100000, ErrorMessage = "Stock quantity must be between 0 and 100,000")]
    public int StockQuantity { get; set; } = 1;
    
    /// <summary>
    /// Gets or sets the product image URL. Must be a valid URL if provided.
    /// </summary>
    [Url(ErrorMessage = "Please provide a valid URL")]
    public string? ImageUrl { get; set; }
    
    /// <summary>
    /// Gets or sets the product release date. Must be a valid date.
    /// </summary>
    [Required(ErrorMessage = "Release date is required")]
    public DateTime ReleaseDate { get; set; }
}