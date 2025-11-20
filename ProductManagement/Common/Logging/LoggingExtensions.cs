namespace ProductManagement.Common.Logging;

public static class LoggingExtensions
{
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
