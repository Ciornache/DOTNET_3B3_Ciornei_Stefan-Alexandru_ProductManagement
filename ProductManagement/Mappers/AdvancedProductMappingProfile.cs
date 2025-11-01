using AutoMapper;
using ProductManagement.Features.Products.DTOs;
using ProductManagement.Features.Products.Resolvers;

namespace ProductManagement.Features.Products.Mappers;

public class AdvancedProductMappingProfile : Profile
{
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
            .ForMember(dest => dest.FormattedPrice, opt => opt.MapFrom<PriceFormattedResolver>())
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => 
                src.Category == ProductCategory.Home ? null : src.ImageUrl))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => 
                src.Category == ProductCategory.Home ? src.Price * 0.9m : src.Price));
    }
}