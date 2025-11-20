using AutoMapper;
using ProductManagement.Features.Products.DTOs;

namespace ProductManagement.Features.Products.Resolvers;

/// <summary>
/// Resolves and formats the product price as a currency string.
/// </summary>
public class PriceFormattedResolver : IValueResolver<Product, ProductProfileDto, string>
{
    /// <summary>
    /// Formats the product price as a currency string with two decimal places.
    /// </summary>
    /// <param name="source">The source Product entity.</param>
    /// <param name="destination">The destination ProductProfileDto.</param>
    /// <param name="destMember">The destination member being resolved.</param>
    /// <param name="context">The resolution context containing the price in Items dictionary.</param>
    /// <returns>A formatted currency string (e.g., "$1,299.99").</returns>
    public string Resolve(Product source, ProductProfileDto destination, string destMember, ResolutionContext context)
    {
        var price = (decimal)context.Items["price"];
        return $"{price:C2}";
    }
}