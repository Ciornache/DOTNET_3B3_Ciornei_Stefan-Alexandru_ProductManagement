using FluentValidation;
using ProductManagement.Persistence;
using ProductManagement.Common.Logging;
using System.Diagnostics;
using AutoMapper;
using ProductManagement.Features.Products.DTOs;

namespace ProductManagement.Features.Products;

/// <summary>
/// Handles the creation of new products with comprehensive validation, logging, and performance tracking.
/// </summary>
public class CreateProductHandler(ProductManagementContext context, ILogger<CreateProductHandler> logger, IValidator<CreateProductProfileCommand> validator, IMapper mapper)
{
    /// <summary>
    /// Processes a product creation request with validation, database persistence, and detailed metrics logging.
    /// </summary>
    /// <param name="command">The command containing product details to create.</param>
    /// <returns>An IResult containing the created ProductProfileDto or validation errors.</returns>
    /// <exception cref="ValidationException">Thrown when product validation fails.</exception>
    public async Task<IResult> Handle(CreateProductProfileCommand command)
    {
        var operationStartTime = Stopwatch.StartNew();
        
        var operationId = Guid.NewGuid().ToString("N")[..8];
        
        using (logger.BeginScope(new Dictionary<string, object>
        {
            ["OperationId"] = operationId,
            ["ProductName"] = command.Name,
            ["Brand"] = command.Brand,
            ["SKU"] = command.SKU,
            ["Category"] = command.Category
        }))
        {
            try
            {
                logger.LogInformation(
                    MyLogEvents.ProductCreationStarted,
                    "Starting product creation operation. OperationId: {OperationId}, Name: {Name}, Brand: {Brand}, SKU: {SKU}, Category: {Category}",
                    operationId, command.Name, command.Brand, command.SKU, command.Category);

                var validationStartTime = Stopwatch.StartNew();
                
                // Log SKU validation
                logger.LogInformation(
                    MyLogEvents.SKUValidationPerformed,
                    "Performing SKU validation for product: {Name}, SKU: {SKU}",
                    command.Name, command.SKU);
                
                // Log stock validation
                logger.LogInformation(
                    MyLogEvents.StockValidationPerformed,
                    "Performing stock validation for product: {Name}, StockQuantity: {StockQuantity}",
                    command.Name, command.StockQuantity);
                
                var validationResult = await validator.ValidateAsync(command);
                
                if (!validationResult.IsValid)
                {
                    validationStartTime.Stop();
                    
                    logger.LogWarning(
                        MyLogEvents.ProductValidationFailed,
                        "Product validation failed. OperationId: {OperationId}, SKU: {SKU}, Name: {Name}, Errors: {Errors}, Duration: {ValidationDuration}ms",
                        operationId, command.SKU, command.Name, 
                        string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)),
                        validationStartTime.ElapsedMilliseconds);
                    
                    throw new ValidationException(validationResult.Errors);
                }
                
                validationStartTime.Stop();
                var validationDuration = validationStartTime.Elapsed;

                var dbStartTime = Stopwatch.StartNew();
                
                logger.LogInformation(
                    MyLogEvents.DatabaseOperationStarted,
                    "Starting database operation for SKU: {SKU}",
                    command.SKU);
                
                var product = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = command.Name,
                    Brand = command.Brand,
                    SKU = command.SKU,
                    Price = command.Price,
                    Category = command.Category,
                    StockQuantity = command.StockQuantity,
                    ImageUrl = command.ImageUrl,
                    IsAvailable = command.StockQuantity > 0,
                    ReleaseDate = command.ReleaseDate,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                context.Products.Add(product);
                await context.SaveChangesAsync();
                
                dbStartTime.Stop();
                var databaseSaveDuration = dbStartTime.Elapsed;
                
                logger.LogInformation(
                    MyLogEvents.DatabaseOperationCompleted,
                    "Database operation completed. ProductId: {ProductId}, SKU: {SKU}, Duration: {Duration}ms",
                    product.Id, command.SKU, dbStartTime.ElapsedMilliseconds);
                
                logger.LogInformation(
                    MyLogEvents.CacheOperationStarted,
                    "Cache invalidation triggered for cache key: {CacheKey}",
                    "all_products");
                
                operationStartTime.Stop();
                var totalDuration = operationStartTime.Elapsed;
                
                var metrics = new ProductCreationMetrics
                {
                    OperationId = operationId,
                    ProductName = product.Name,
                    SKU = product.SKU,
                    ProductCategory = product.Category,
                    ValidationDuration = validationDuration,
                    DatabaseSaveDuration = databaseSaveDuration,
                    TotalDuration = totalDuration,
                    Success = true,
                    ErrorReason = null
                };
                
                logger.LogProductCreationMetrics(metrics);

                // Map the Product entity to ProductProfileDto
                var productDto = mapper.Map<ProductProfileDto>(product);

                return Results.Created($"/products/{product.Id}", productDto);
            }
            catch (ValidationException ex)
            {
                operationStartTime.Stop();
                
                var errorMetrics = new ProductCreationMetrics
                {
                    OperationId = operationId,
                    ProductName = command.Name,
                    SKU = command.SKU,
                    ProductCategory = command.Category,
                    ValidationDuration = TimeSpan.Zero,
                    DatabaseSaveDuration = TimeSpan.Zero,
                    TotalDuration = operationStartTime.Elapsed,
                    Success = false,
                    ErrorReason = $"Validation failed: {string.Join("; ", ex.Errors.Select(e => e.ErrorMessage))}"
                };
                
                logger.LogProductCreationMetrics(errorMetrics);
                
                throw;
            }
            catch (Exception ex)
            {
                operationStartTime.Stop();
                
                var errorMetrics = new ProductCreationMetrics
                {
                    OperationId = operationId,
                    ProductName = command.Name,
                    SKU = command.SKU,
                    ProductCategory = command.Category,
                    ValidationDuration = TimeSpan.Zero,
                    DatabaseSaveDuration = TimeSpan.Zero,
                    TotalDuration = operationStartTime.Elapsed,
                    Success = false,
                    ErrorReason = ex.Message
                };
                
                logger.LogProductCreationMetrics(errorMetrics);
                
                throw;
            }
        }
    }
}