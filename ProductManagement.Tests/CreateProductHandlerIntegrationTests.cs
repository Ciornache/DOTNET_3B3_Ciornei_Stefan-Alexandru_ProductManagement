using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ProductManagement.Features.Products;
using ProductManagement.Features.Products.DTOs;
using ProductManagement.Common.Mapping;
using ProductManagement.Common.Logging;
using ProductManagement.Persistence;
using ProductManagement.Validators;
using Xunit;

namespace ProductManagement.Tests;

/// <summary>
/// Integration tests for the CreateProductHandler to verify product creation functionality with validation, mapping, and logging.
/// </summary>
public class CreateProductHandlerIntegrationTests : IDisposable
{
    private readonly ProductManagementContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly Mock<ILogger<CreateProductHandler>> _loggerMock;
    private readonly CreateProductHandler _handler;

    /// <summary>
    /// Initializes a new instance of the test class with in-memory database, AutoMapper configuration, cache, and mocked dependencies.
    /// </summary>
    public CreateProductHandlerIntegrationTests()
    {
        var databaseName = $"ProductTestDb_{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<ProductManagementContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;
        
        _context = new ProductManagementContext(options);

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProductMappingProfile>();
            cfg.AddProfile<AdvancedProductMappingProfile>();
        }, NullLoggerFactory.Instance);
        _mapper = mapperConfig.CreateMapper();

        _cache = new MemoryCache(new MemoryCacheOptions());

        _loggerMock = new Mock<ILogger<CreateProductHandler>>();

        var validatorLoggerMock = new Mock<ILogger<CreateProductProfileValidator>>();

        var validator = new CreateProductProfileValidator(validatorLoggerMock.Object, _context);

        _handler = new CreateProductHandler(_context, _loggerMock.Object, validator, _mapper);
    }

    /// <summary>
    /// Tests that a valid electronics product request successfully creates a product with correct mappings including
    /// category display name, brand initials, product age, formatted price, and availability status.
    /// </summary>
    /// <returns>A task representing the asynchronous test operation.</returns>
    [Fact]
    public async Task Handle_ValidElectronicsProductRequest_CreatesProductWithCorrectMappings()
    {
        var request = new CreateProductProfileCommand
        {
            Name = "Smart Device Pro",
            Brand = "Tech Masters",
            SKU = "PREM-TECH-LAPTOP-24",
            Price = 1299.99m, 
            Category = ProductCategory.Electronics,
            StockQuantity = 15, 
            ImageUrl = "https://example.com/laptop.jpg",
            ReleaseDate = DateTime.UtcNow.AddMonths(-6) 
        };

        var result = await _handler.Handle(request);

        Assert.NotNull(result);
        var createdResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Created<ProductProfileDto>>(result);
        var productProfileDto = createdResult.Value;

        Assert.NotNull(productProfileDto);
        Assert.Equal(request.Name, productProfileDto.Name);
        Assert.Equal(request.Brand, productProfileDto.Brand);
        Assert.Equal(request.SKU, productProfileDto.SKU);

        Assert.Equal("Electronics & Technology", productProfileDto.CategoryDisplayName);
        
        Assert.Equal("TM", productProfileDto.BrandInitials);
        
        Assert.NotNull(productProfileDto.ProductAge);
        Assert.Contains("month", productProfileDto.ProductAge, StringComparison.OrdinalIgnoreCase);
        
        Assert.StartsWith("$", productProfileDto.FormattedPrice);
        Assert.Contains("1,299.99", productProfileDto.FormattedPrice); // Price includes comma separator
        
        Assert.Equal("In Stock", productProfileDto.AvailabilityStatus);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                MyLogEvents.ProductCreationStarted,
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Starting product creation operation")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    /// <summary>
    /// Tests that attempting to create a product with a duplicate SKU throws a ValidationException
    /// and logs the validation failure event.
    /// </summary>
    /// <returns>A task representing the asynchronous test operation.</returns>
    [Fact]
    public async Task Handle_DuplicateSKU_ThrowsValidationExceptionWithLogging()
    {
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

        var exception = await Assert.ThrowsAsync<ValidationException>(
            async () => await _handler.Handle(request));

        Assert.Contains("already exists", exception.Message);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                MyLogEvents.ProductValidationFailed,
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Product validation failed")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    /// <summary>
    /// Tests that a Home category product applies the 10% discount to the price and sets ImageUrl to null
    /// as part of conditional mapping based on product category.
    /// </summary>
    /// <returns>A task representing the asynchronous test operation.</returns>
    [Fact]
    public async Task Handle_HomeProductRequest_AppliesDiscountAndConditionalMapping()
    {
        var request = new CreateProductProfileCommand
        {
            Name = "Wooden Coffee Table", 
            Brand = "Home Essentials",
            SKU = "HOME-TABLE-2024",
            Price = 150.00m,
            Category = ProductCategory.Home,
            StockQuantity = 20,
            ImageUrl = "https://example.com/table.jpg",
            ReleaseDate = DateTime.UtcNow.AddMonths(-2)
        };

        var result = await _handler.Handle(request);

        Assert.NotNull(result);
        var createdResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Created<ProductProfileDto>>(result);
        var productProfileDto = createdResult.Value;

        Assert.NotNull(productProfileDto);
        
        Assert.Equal(request.Name, productProfileDto.Name);
        Assert.Equal(request.Brand, productProfileDto.Brand);
        Assert.Equal(request.SKU, productProfileDto.SKU);
        
        Assert.Equal("Home & Garden", productProfileDto.CategoryDisplayName);
        
        Assert.Equal("$135.00", productProfileDto.FormattedPrice);
        
        Assert.Null(productProfileDto.ImageUrl);
    }

    /// <summary>
    /// Cleans up test resources by deleting the in-memory database and disposing of the context and cache.
    /// </summary>
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        _cache.Dispose();
    }
}
