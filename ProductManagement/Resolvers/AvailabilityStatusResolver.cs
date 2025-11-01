using AutoMapper;
using ProductManagement.Features.Products.DTOs;

namespace ProductManagement.Features.Products.Resolvers;

public class AvailabilityStatusResolver : IValueResolver<Product, ProductProfileDto, string>
{
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