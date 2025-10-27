using ProductManagement.Features.Products.DTOs;
using AutoMapper;

namespace ProductManagement.Features.Products.Resolvers;

public class CategoryDisplayResolver : IValueResolver<Product, ProductProfileDto, string>
{
    public string Resolve(Product source, ProductProfileDto destination, string destMember, ResolutionContext context)
    {
        switch (source.Category)
        {
            case ProductCategory.Electronics:
                return "Electronics & Technology";
            case ProductCategory.Clothing:
                return "Clothing & Fashion";
            case ProductCategory.Books:
                return "Books & Media";
            case ProductCategory.Home:
                return "Home & Garden";
            default:
                return "Uncategorized";
        }
    }
}