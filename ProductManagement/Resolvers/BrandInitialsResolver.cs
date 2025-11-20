using AutoMapper;
using MediatR.NotificationPublishers;
using ProductManagement.Features.Products.DTOs;

namespace ProductManagement.Features.Products.Resolvers;

/// <summary>
/// Resolves the brand initials by extracting the first letter of each word in the brand name.
/// </summary>
public class BrandInitialsResolver : IValueResolver<Product, ProductProfileDto, string>
{
    /// <summary>
    /// Extracts and returns the initials from the brand name.
    /// </summary>
    /// <param name="source">The source Product entity.</param>
    /// <param name="destination">The destination ProductProfileDto.</param>
    /// <param name="destMember">The destination member being resolved.</param>
    /// <param name="context">The resolution context.</param>
    /// <returns>A string containing the uppercase initials of the brand, or "?" if brand is null.</returns>
    public string Resolve(Product source, ProductProfileDto destination, string destMember, ResolutionContext context)
    {
        if (source.Brand != null)
        {
            var words = source.Brand.Split(" ");
            var initials = "";
            foreach (var word in words)
                initials += word[0].ToString().ToUpper();
            return initials;
        }
        return "?";
    }
}