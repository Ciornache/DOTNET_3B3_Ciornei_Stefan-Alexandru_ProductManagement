using AutoMapper;
using ProductManagement.Features.Products;
using ProductManagement.Features.Products.DTOs;
using ProductManagement.Features.Products.Resolvers;

namespace ProductManagement.Common.Mapping;

/// <summary>
/// Defines advanced AutoMapper mappings for products with custom resolvers and conditional logic.
/// </summary>
public class AdvancedProductMappingProfile : Profile
{
    /// <summary>
    /// Initializes advanced mapping configuration including custom resolvers for category-specific transformations.
    /// </summary>
    public AdvancedProductMappingProfile()
    {
        CreateMap<CreateProductProfileCommand, Product>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.StockQuantity > 0))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        
        CreateMap<Product, ProductProfileDto>()
            .ForMember(dest => dest.CategoryDisplayName, opt => opt.MapFrom<CategoryDisplayResolver>())
            .ForMember(dest => dest.AvailabilityStatus, opt => opt.MapFrom<AvailabilityStatusResolver>())
            .ForMember(dest => dest.BrandInitials, opt => opt.MapFrom<BrandInitialsResolver>())
            .ForMember(dest => dest.ProductAge, opt => opt.MapFrom<ProductAgeResolver>())
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => 
                src.Category == ProductCategory.Home ? null : src.ImageUrl))
            .ForMember(dest => dest.FormattedPrice, opt => opt.MapFrom(src => 
                (src.Category == ProductCategory.Home ? src.Price * 0.9m : src.Price).ToString("C2")));
    }
}