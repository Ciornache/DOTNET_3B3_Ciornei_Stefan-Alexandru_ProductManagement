using AutoMapper;
using ProductManagement.Features.Products.DTOs;

namespace ProductManagement.Features.Products.Resolvers;

public class ProductAgeResolver : IValueResolver<Product, ProductProfileDto, string>
{
    public string Resolve(Product source, ProductProfileDto destination, string destMember, ResolutionContext context)
    {
        var window = DateTime.Now - source.CreatedAt;
        if (window < TimeSpan.FromDays(30))
            return $"New Release"; 
        if (source.CreatedAt - DateTime.Now < TimeSpan.FromDays(365))
            return $"{window.TotalDays / 30} months old";
        if (source.CreatedAt - DateTime.Now < TimeSpan.FromDays(1825))
            return $"{window.TotalDays / 365} years old";
        return "Classic";
    }
}