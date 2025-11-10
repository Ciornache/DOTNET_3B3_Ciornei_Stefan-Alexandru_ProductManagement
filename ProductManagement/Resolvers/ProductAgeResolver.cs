using AutoMapper;
using ProductManagement.Features.Products.DTOs;

namespace ProductManagement.Features.Products.Resolvers;

public class ProductAgeResolver : IValueResolver<Product, ProductProfileDto, string>
{
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