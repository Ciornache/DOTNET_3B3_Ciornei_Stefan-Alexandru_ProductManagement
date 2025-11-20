using ProductManagement.Features.Products.DTOs;
using AutoMapper;

namespace ProductManagement.Features.Products.Resolvers;

/// <summary>
/// Resolves the display name for product categories with user-friendly formatting.
/// </summary>
public class CategoryDisplayResolver : IValueResolver<Product, ProductProfileDto, string>
{
    /// <summary>
    /// Converts a ProductCategory enum value to a formatted display string.
    /// </summary>
    /// <param name="source">The source Product entity.</param>
    /// <param name="destination">The destination ProductProfileDto.</param>
    /// <param name="destMember">The destination member being resolved.</param>
    /// <param name="context">The resolution context.</param>
    /// <returns>A formatted category display name (e.g., "Electronics & Technology", "Home & Garden").</returns>
    public string Resolve(Product source, ProductProfileDto destination, string destMember, ResolutionContext context)
    {
        switch (source.Category)
        {
            case ProductCategory.Electronics:
                return "Electronics & Technology";
            case ProductCategory.Clothing:
                return "Clothing & Fashion";
            case ProductCategory.Books:
                return "Books & Media";
            case ProductCategory.Home:
                return "Home & Garden";
            default:
                return "Uncategorized";
        }
    }
}