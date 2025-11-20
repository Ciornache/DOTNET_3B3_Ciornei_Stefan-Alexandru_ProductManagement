namespace ProductManagement.Common.Logging;

/// <summary>
/// Provides extension methods for logging product-related operations and metrics.
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    /// Logs comprehensive metrics for product creation operations including timing and success status.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="metrics">The product creation metrics to log.</param>
    public static void LogProductCreationMetrics(
        this ILogger logger,
        ProductCreationMetrics metrics)
    {
        if (metrics.Success)
        {
            logger.LogInformation(
                MyLogEvents.ProductCreateCompleted,
                "Product creation completed successfully. " +
                "OperationId: {OperationId}, Name: {Name}, SKU: {SKU}, Category: {Category}, " +
                "ValidationDuration: {ValidationDurationMs}ms, DatabaseSaveDuration: {DatabaseSaveDurationMs}ms, " +
                "TotalDuration: {TotalDurationMs}ms, Success: {Success}",
                metrics.OperationId,
                metrics.ProductName,
                metrics.SKU,
                metrics.ProductCategory,
                metrics.ValidationDuration.TotalMilliseconds,
                metrics.DatabaseSaveDuration.TotalMilliseconds,
                metrics.TotalDuration.TotalMilliseconds,
                metrics.Success);
        }
        else
        {
            logger.LogError(
                "Product creation failed. " +
                "OperationId: {OperationId}, Name: {Name}, SKU: {SKU}, Category: {Category}, " +
                "ValidationDuration: {ValidationDurationMs}ms, DatabaseSaveDuration: {DatabaseSaveDurationMs}ms, " +
                "TotalDuration: {TotalDurationMs}ms, Success: {Success}, ErrorReason: {ErrorReason}",
                metrics.OperationId,
                metrics.ProductName,
                metrics.SKU,
                metrics.ProductCategory,
                metrics.ValidationDuration.TotalMilliseconds,
                metrics.DatabaseSaveDuration.TotalMilliseconds,
                metrics.TotalDuration.TotalMilliseconds,
                metrics.Success,
                metrics.ErrorReason);
        }
    }
}
