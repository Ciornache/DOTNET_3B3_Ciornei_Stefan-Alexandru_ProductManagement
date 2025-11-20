using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ProductManagement.Features.Products;
using ProductManagement.Features.Products.DTOs;
using ProductManagement.Features.Products.Mappers;
using ProductManagement.Mappers;
using ProductManagement.LogEventsConstants;
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

    public CreateProductHandlerIntegrationTests()
    {
        // Set up in-memory database with unique name
        var databaseName = $"ProductTestDb_{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<ProductManagementContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
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

        // Create validator with dependencies
        var validator = new CreateProductProfileValidator(validatorLoggerMock.Object, _context);

        // Create handler instance with all dependencies (now including IMapper)
        _handler = new CreateProductHandler(_context, _loggerMock.Object, validator, _mapper);
    }

    [Fact]
    public async Task Handle_ValidElectronicsProductRequest_CreatesProductWithCorrectMappings()
    {
        // Arrange: Create valid Electronics product request with all properties
        var request = new CreateProductProfileCommand
        {
            Name = "Smart Device Pro", // Contains "Smart" - a technology keyword
            Brand = "Tech Masters",
            SKU = "PREM-TECH-LAPTOP-24", // Starts with PREM- for premium product
            Price = 1299.99m, // Price > 500 requires PREM- prefix
            Category = ProductCategory.Electronics,
            StockQuantity = 15, // ≤ 20 for expensive products
            ImageUrl = "https://example.com/laptop.jpg",
            ReleaseDate = DateTime.UtcNow.AddMonths(-6) // Within 5 years
        };

        // Act: Call handler
        var result = await _handler.Handle(request);

        // Assert: Verify Created result type with ProductProfileDto
        Assert.NotNull(result);
        var createdResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Created<ProductProfileDto>>(result);
        var productProfileDto = createdResult.Value;

        // Assert: Check basic properties and advanced mappings on the DTO
        Assert.NotNull(productProfileDto);
        Assert.Equal(request.Name, productProfileDto.Name);
        Assert.Equal(request.Brand, productProfileDto.Brand);
        Assert.Equal(request.SKU, productProfileDto.SKU);

        // Assert: Check CategoryDisplayName = "Electronics & Technology"
        Assert.Equal("Electronics & Technology", productProfileDto.CategoryDisplayName);
        
        // Assert: Check BrandInitials for two-word brand "Tech Masters" -> "TM"
        Assert.Equal("TM", productProfileDto.BrandInitials);
        
        // Assert: Check ProductAge calculation (should contain time information)
        Assert.NotNull(productProfileDto.ProductAge);
        Assert.Contains("month", productProfileDto.ProductAge, StringComparison.OrdinalIgnoreCase);
        
        // Assert: Check FormattedPrice starts with currency symbol
        Assert.StartsWith("$", productProfileDto.FormattedPrice);
        Assert.Contains("1,299.99", productProfileDto.FormattedPrice); // Price includes comma separator
        
        // Assert: Check AvailabilityStatus based on stock
        Assert.Equal("In Stock", productProfileDto.AvailabilityStatus);

        // Assert: Verify ProductCreationStarted log called once
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
            Name = "Wooden Coffee Table", // Appropriate name for home products
            Brand = "Home Essentials",
            SKU = "HOME-TABLE-2024",
            Price = 150.00m, // Price ≤ $200 for Home category
            Category = ProductCategory.Home,
            StockQuantity = 20, // ≤ 20 for expensive products (>$100)
            ImageUrl = "https://example.com/table.jpg",
            ReleaseDate = DateTime.UtcNow.AddMonths(-2)
        };

        // Act: Call handler
        var result = await _handler.Handle(request);

        // Assert: Verify Created result type with ProductProfileDto
        Assert.NotNull(result);
        var createdResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Created<ProductProfileDto>>(result);
        var productProfileDto = createdResult.Value;

        Assert.NotNull(productProfileDto);
        
        // The DTO should have the original product data transformed
        Assert.Equal(request.Name, productProfileDto.Name);
        Assert.Equal(request.Brand, productProfileDto.Brand);
        Assert.Equal(request.SKU, productProfileDto.SKU);
        
        // Assert: Check CategoryDisplayName = "Home & Garden"
        Assert.Equal("Home & Garden", productProfileDto.CategoryDisplayName);
        
        // Assert: Check Price has 10% discount applied (in the DTO mapping)
        // 150 * 0.9 = 135.00
        Assert.Equal("$135.00", productProfileDto.FormattedPrice);
        
        // Assert: Check ImageUrl is null (content filtering for Home category)
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
