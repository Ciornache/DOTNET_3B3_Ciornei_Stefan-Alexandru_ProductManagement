namespace ProductManagement.Common.Logging;

/// <summary>
/// Defines event IDs for structured logging of product-related operations.
/// </summary>
public class MyLogEvents
{
    /// <summary>
    /// Event ID for when a product creation operation starts.
    /// </summary>
    public const int ProductCreationStarted = 2001;
    
    /// <summary>
    /// Event ID for when product validation fails.
    /// </summary>
    public const int ProductValidationFailed = 2002;
    
    /// <summary>
    /// Event ID for when product creation completes successfully.
    /// </summary>
    public const int ProductCreateCompleted = 2003;
    
    /// <summary>
    /// Event ID for when a database operation starts.
    /// </summary>
    public const int DatabaseOperationStarted = 2004;
    
    /// <summary>
    /// Event ID for when a database operation completes.
    /// </summary>
    public const int DatabaseOperationCompleted = 2005;
    
    /// <summary>
    /// Event ID for when a cache operation starts.
    /// </summary>
    public const int CacheOperationStarted = 2006;
    
    /// <summary>
    /// Event ID for when SKU validation is performed.
    /// </summary>
    public const int SKUValidationPerformed = 2007;
    
    /// <summary>
    /// Event ID for when stock validation is performed.
    /// </summary>
    public const int StockValidationPerformed = 2008;
}