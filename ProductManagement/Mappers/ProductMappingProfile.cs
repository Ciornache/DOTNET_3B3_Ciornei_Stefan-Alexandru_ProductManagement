using AutoMapper;
using ProductManagement.Features.Products.DTOs;

namespace ProductManagement.Mappers;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Features.Products.Product, ProductProfileDto>();
        CreateMap<Features.Products.CreateProductProfileCommand, Features.Products.Product>();
    }
}
