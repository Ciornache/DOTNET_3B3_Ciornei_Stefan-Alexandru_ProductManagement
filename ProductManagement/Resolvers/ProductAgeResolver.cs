using AutoMapper;
using ProductManagement.Features.Products.DTOs;

namespace ProductManagement.Features.Products.Resolvers;

/// <summary>
/// Resolves the age of a product based on its release date.
/// </summary>
public class ProductAgeResolver : IValueResolver<Product, ProductProfileDto, string>
{
    /// <summary>
    /// Calculates and returns a human-readable age description for the product.
    /// </summary>
    /// <param name="source">The source Product entity.</param>
    /// <param name="destination">The destination ProductProfileDto.</param>
    /// <param name="destMember">The destination member being resolved.</param>
    /// <param name="context">The resolution context.</param>
    /// <returns>A string describing the product age ("New Release", "X months old", "X years old", or "Classic").</returns>
    public string Resolve(Product source, ProductProfileDto destination, string destMember, ResolutionContext context)
    {
        var window = DateTime.Now - source.ReleaseDate; // Correctly use ReleaseDate instead of CreatedAt
        if (window < TimeSpan.FromDays(30))
            return "New Release"; 
        if (window < TimeSpan.FromDays(365))
            return $"{Math.Round(window.TotalDays / 30)} months old";
        if (window < TimeSpan.FromDays(1825)) // 5 years
            return $"{Math.Round(window.TotalDays / 365)} years old";
        return "Classic";
    }
}