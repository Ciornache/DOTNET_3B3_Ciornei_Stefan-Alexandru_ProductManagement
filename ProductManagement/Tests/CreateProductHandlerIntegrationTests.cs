using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ProductManagement.Features.Products;
using ProductManagement.Features.Products.DTOs;
using ProductManagement.Features.Products.Mappers;
using ProductManagement.LogEventsConstants;
using ProductManagement.Mappers;
using ProductManagement.Persistence;
using ProductManagement.Validators;
using Xunit;

namespace ProductManagement.Tests;

public class CreateProductHandlerIntegrationTests : IDisposable
{
    private readonly ProductManagementContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly Mock<ILogger<CreateProductHandler>> _loggerMock;
    private readonly CreateProductHandler _handler;
    private readonly string _databaseName;

    public CreateProductHandlerIntegrationTests()
    {
        // Set up in-memory database with unique name
        _databaseName = $"ProductTestDb_{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<ProductManagementContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;
        
        _context = new ProductManagementContext(options);

        // Configure AutoMapper with both product profiles
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProductMappingProfile>();
            cfg.AddProfile<AdvancedProductMappingProfile>();
        }, NullLoggerFactory.Instance);
        _mapper = mapperConfig.CreateMapper();

        // Set up memory cache
        _cache = new MemoryCache(new MemoryCacheOptions());

        // Mock ILogger<CreateProductHandler>
        _loggerMock = new Mock<ILogger<CreateProductHandler>>();

        // Mock ILogger<CreateProductProfileValidator>
        var validatorLoggerMock = new Mock<ILogger<CreateProductProfileValidator>>();

        // Create validator with dependencies (logger first, then context)
        var validator = new CreateProductProfileValidator(validatorLoggerMock.Object, _context);

        // Create handler instance with all dependencies
        _handler = new CreateProductHandler(_context, _loggerMock.Object, validator);
    }

    [Fact]
    public async Task Handle_ValidElectronicsProductRequest_CreatesProductWithCorrectMappings()
    {
        // Arrange: Create valid Electronics product request with all properties
        var request = new CreateProductProfileCommand
        {
            Name = "Smart Device Pro",
            Brand = "Tech Masters",
            SKU = "PREM-TECH-LAPTOP-24", // SKU updated to meet premium product rule (Price > 500)
            Price = 1299.99m,
            Category = ProductCategory.Electronics,
            StockQuantity = 15,
            ImageUrl = "https://example.com/laptop.jpg",
            ReleaseDate = DateTime.UtcNow.AddYears(-1) // Updated to be clearly within the 5-year rule
        };

        // Act: Call handler
        var result = await _handler.Handle(request);

        // Assert: Verify Created result type
        Assert.NotNull(result);
        var createdResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Created<Product>>(result);
        var product = createdResult.Value;

        // Assert: Check basic properties on the created entity
        Assert.NotNull(product);
        Assert.Equal(request.Name, product.Name);
        Assert.Equal(request.Brand, product.Brand);
        Assert.Equal(request.SKU, product.SKU);

        // Now, test the advanced mappings by mapping the created product to the DTO
        var productProfileDto = _mapper.Map<ProductProfileDto>(product);

        // Assert: Check advanced and conditional mappings
        Assert.Equal("Electronics & Technology", productProfileDto.CategoryDisplayName);
        Assert.Equal("TM", productProfileDto.BrandInitials); // For "Tech Masters"
        Assert.Contains("months", productProfileDto.ProductAge); // ProductAge is dynamic, check for keyword
        Assert.StartsWith("$", productProfileDto.FormattedPrice); // Check for currency symbol
        Assert.Equal("In Stock", productProfileDto.AvailabilityStatus); // Stock is > 0

        // Verify ProductCreationStarted log called once
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                MyLogEvents.ProductCreationStarted,
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Starting product creation operation")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateSKU_ThrowsValidationExceptionWithLogging()
    {
        // Arrange: Create existing product in database with specific SKU
        var existingProduct = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Existing Product",
            Brand = "Existing Brand",
            SKU = "DUPLICATE-SKU-123",
            Price = 99.99m,
            Category = ProductCategory.Electronics,
            StockQuantity = 10,
            ImageUrl = "https://example.com/existing.jpg",
            IsAvailable = true,
            ReleaseDate = DateTime.UtcNow.AddMonths(-3),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Products.Add(existingProduct);
        await _context.SaveChangesAsync();

        // Arrange: Create request with same SKU
        var request = new CreateProductProfileCommand
        {
            Name = "New Product",
            Brand = "New Brand",
            SKU = "DUPLICATE-SKU-123",
            Price = 149.99m,
            Category = ProductCategory.Electronics,
            StockQuantity = 5,
            ImageUrl = "https://example.com/new.jpg",
            ReleaseDate = DateTime.UtcNow
        };

        // Act & Assert: Verify ValidationException thrown
        var exception = await Assert.ThrowsAsync<ValidationException>(
            async () => await _handler.Handle(request));

        // Assert: Check exception message contains "already exists"
        Assert.Contains("already exists", exception.Message);

        // Assert: Verify ProductValidationFailed log called once
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                MyLogEvents.ProductValidationFailed,
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Product validation failed")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_HomeProductRequest_AppliesDiscountAndConditionalMapping()
    {
        // Arrange: Create valid Home product request
        var request = new CreateProductProfileCommand
        {
            Name = "Wooden Coffee Table",
            Brand = "Home Essentials",
            SKU = "HOME-TABLE-2024",
            Price = 150.00m,
            Category = ProductCategory.Home,
            StockQuantity = 20, // Changed from 25 to 20 to satisfy the stock limit for expensive products
            ImageUrl = "https://example.com/table.jpg",
            ReleaseDate = DateTime.UtcNow.AddMonths(-2)
        };

        // Act: Call handler
        var result = await _handler.Handle(request);

        // Assert: Verify Created result type
        Assert.NotNull(result);
        var createdResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Created<Product>>(result);
        var product = createdResult.Value;

        Assert.NotNull(product);
        
        // The actual Product entity should store the original price
        // The discount is applied at the mapping/DTO level, not at entity level
        Assert.Equal(150.00m, product.Price);
        Assert.Equal(ProductCategory.Home, product.Category);
        
        // Now let's test the mapping with AutoMapper
        var productProfileDto = _mapper.Map<ProductProfileDto>(product);
        
        // Assert: Check CategoryDisplayName = "Home & Garden"
        Assert.Equal("Home & Garden", productProfileDto.CategoryDisplayName);
        
        // Assert: Check Price has 10% discount applied (in the DTO mapping)
        Assert.Equal("$135.00", productProfileDto.FormattedPrice); // 150 * 0.9 = 135
        
        // Assert: Check ImageUrl is null (content filtering)
        Assert.Null(productProfileDto.ImageUrl);
    }

    public void Dispose()
    {
        // Proper disposal of context and cache
        _context.Database.EnsureDeleted();
        _context.Dispose();
        _cache.Dispose();
    }
}
