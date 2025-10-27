using AutoMapper;
using MediatR;
using ProductManagement.Features.Products.DTOs;
using ProductManagement.Persistence;

namespace ProductManagement.Features.Products;

public class CreateProductProfileCommandHandler : IRequestHandler<CreateProductProfileCommand, ProductProfileDto>
{
    private readonly ProductManagementContext _context;
    private readonly IMapper _mapper;
    
    public CreateProductProfileCommandHandler(ProductManagementContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<ProductProfileDto> Handle(CreateProductProfileCommand request, CancellationToken cancellationToken)
    {
        // Implementation will go here
        throw new NotImplementedException();
    }
}