using AutoMapper;
using ProductManagement.Features.Products.DTOs;

namespace ProductManagement.Features.Products.Resolvers;

public class PriceFormattedResolver : IValueResolver<Product, ProductProfileDto, string>
{
    public string Resolve(Product source, ProductProfileDto destination, string destMember, ResolutionContext context)
    {
        return $"{source.Price:C2}";
    }
}