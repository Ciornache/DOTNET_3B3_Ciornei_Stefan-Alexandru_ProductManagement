using AutoMapper;
using ProductManagement.Features.Products.DTOs;

namespace ProductManagement.Features.Products.Resolvers;

public class PriceFormattedResolver : IValueResolver<Product, ProductProfileDto, string>
{
    public string Resolve(Product source, ProductProfileDto destination, string destMember, ResolutionContext context)
    {
        // The discount is now applied in the main mapping profile, 
        // this resolver is only for formatting.
        var price = (decimal)context.Items["price"];
        return $"{price:C2}";
    }
}