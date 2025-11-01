using AutoMapper;
using MediatR.NotificationPublishers;
using ProductManagement.Features.Products.DTOs;

namespace ProductManagement.Features.Products.Resolvers;

public class BrandInitialsResolver : IValueResolver<Product, ProductProfileDto, string>
{
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