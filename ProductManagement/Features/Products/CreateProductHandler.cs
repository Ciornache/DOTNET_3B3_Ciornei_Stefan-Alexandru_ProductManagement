using FluentValidation;
using ProductManagement.Persistence;
using ProductManagement.LogEventsConstants;
using System.Diagnostics;

namespace ProductManagement.Features.Products;

public class CreateProductHandler(ProductManagementContext context, ILogger<CreateProductHandler> logger, IValidator<CreateProductProfileCommand> validator)
{
    public async Task<IResult> Handle(CreateProductProfileCommand command)
    {
        // Track operation start time
        var operationStartTime = Stopwatch.StartNew();
        
        // Generate unique operation ID (8 characters)
        var operationId = Guid.NewGuid().ToString("N")[..8];
        
        // Use logging scope for entire product operation
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
                
                logger.LogInformation(
                    MyLogEvents.StockValidationPerformed,
                    "Performing stock validation for product: {Name}, StockQuantity: {StockQuantity}",
                    command.Name, command.StockQuantity);
                
                var validationResult = await validator.ValidateAsync(command);
                
                if (!validationResult.IsValid)
                {
                    validationStartTime.Stop();
                    
                    // Log validation failures with product-specific details
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

                // Time database operations separately
                var dbStartTime = Stopwatch.StartNew();
                
                // Log database operation start
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
                
                // Log database operation completion with ProductId
                logger.LogInformation(
                    MyLogEvents.DatabaseOperationCompleted,
                    "Database operation completed. ProductId: {ProductId}, SKU: {SKU}, Duration: {Duration}ms",
                    product.Id, command.SKU, dbStartTime.ElapsedMilliseconds);
                
                // Log cache operations with "all_products" cache key
                logger.LogInformation(
                    MyLogEvents.CacheOperationStarted,
                    "Cache invalidation triggered for cache key: {CacheKey}",
                    "all_products");
                
                // Calculate total operation duration
                operationStartTime.Stop();
                var totalDuration = operationStartTime.Elapsed;
                
                // Log comprehensive ProductCreationMetrics for success cases
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
                
                logger.LogInformation(
                    MyLogEvents.ProductCreateCompleted,
                    "Product creation completed successfully. {@ProductCreationMetrics}",
                    metrics);

                return Results.Created($"/products/{product.Id}", product);
            }
            catch (ValidationException ex)
            {
                // Log error metrics in catch block with product details
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
                
                logger.LogError(
                    "Product creation failed due to validation. {@ProductCreationMetrics}",
                    errorMetrics);
                
                // Re-throw exception for global handler
                throw;
            }
            catch (Exception ex)
            {
                // Log error metrics in catch block with product details
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
                
                logger.LogError(
                    ex,
                    "Product creation failed with exception. {@ProductCreationMetrics}",
                    errorMetrics);
                
                // Re-throw exception for global handler
                throw;
            }
        }
    }
}