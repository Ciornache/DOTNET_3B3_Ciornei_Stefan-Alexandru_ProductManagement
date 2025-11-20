using AutoMapper;
using ProductManagement.Features.Products.DTOs;

namespace ProductManagement.Common.Mapping;

/// <summary>
/// Defines basic AutoMapper mappings for Product entities and related DTOs/commands.
/// </summary>
public class ProductMappingProfile : Profile
{
    /// <summary>
    /// Initializes the mapping configuration for products.
    /// </summary>
    public ProductMappingProfile()
    {
        CreateMap<Features.Products.Product, ProductProfileDto>();
        CreateMap<Features.Products.CreateProductProfileCommand, Features.Products.Product>();
    }
}
