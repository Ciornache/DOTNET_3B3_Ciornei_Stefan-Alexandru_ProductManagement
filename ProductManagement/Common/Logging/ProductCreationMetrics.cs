using ProductManagement.Features.Products;

namespace ProductManagement.Common.Logging;

/// <summary>
/// Record representing comprehensive metrics collected during product creation operations.
/// </summary>
public record ProductCreationMetrics
{
    /// <summary>
    /// Gets the unique identifier for this operation instance.
    /// </summary>
    public required string OperationId { get; init; }
    
    /// <summary>
    /// Gets the name of the product being created.
    /// </summary>
    public required string ProductName { get; init; }
    
    /// <summary>
    /// Gets the SKU of the product being created.
    /// </summary>
    public required string SKU { get; init; }
    
    /// <summary>
    /// Gets the category of the product being created.
    /// </summary>
    public required ProductCategory ProductCategory { get; init; }
    
    /// <summary>
    /// Gets the time spent on validation operations.
    /// </summary>
    public required TimeSpan ValidationDuration { get; init; }
    
    /// <summary>
    /// Gets the time spent on database save operations.
    /// </summary>
    public required TimeSpan DatabaseSaveDuration { get; init; }
    
    /// <summary>
    /// Gets the total duration of the product creation operation.
    /// </summary>
    public required TimeSpan TotalDuration { get; init; }
    
    /// <summary>
    /// Gets whether the product creation operation was successful.
    /// </summary>
    public required bool Success { get; init; }
    
    /// <summary>
    /// Gets the error reason if the operation failed. Null if successful.
    /// </summary>
    public string? ErrorReason { get; init; }
}
