using AutoMapper;
using ProductManagement.Features.Products.DTOs;

namespace ProductManagement.Features.Products.Resolvers;

/// <summary>
/// Resolves the availability status of a product based on stock quantity and availability flag.
/// </summary>
public class AvailabilityStatusResolver : IValueResolver<Product, ProductProfileDto, string>
{
    /// <summary>
    /// Resolves the product availability status string based on stock levels.
    /// </summary>
    /// <param name="source">The source Product entity.</param>
    /// <param name="destination">The destination ProductProfileDto.</param>
    /// <param name="destMember">The destination member being resolved.</param>
    /// <param name="context">The resolution context.</param>
    /// <returns>A string representing the availability status ("Not Available", "Unavailable", "Last Item", "Limited Stock", or "In Stock").</returns>
    public string Resolve(Product source, ProductProfileDto destination, string destMember, ResolutionContext context)
    {
        if(!source.IsAvailable)
            return "Not Available";
        switch (source.StockQuantity)
        {
            case 0:
                return "Unavailable";
            case 1:
                return "Last Item";
            case > 5:
                return "In Stock";
            case <= 5:
                return "Limited Stock"; 
        }
    }
}