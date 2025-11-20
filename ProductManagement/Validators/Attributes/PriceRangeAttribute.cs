using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Validators.Attributes;

/// <summary>
/// Validates that a price value falls within a specified range.
/// </summary>
/// <param name="minPrice">The minimum allowed price.</param>
/// <param name="maxPrice">The maximum allowed price.</param>
public class PriceRangeAttribute(float minPrice, float maxPrice) : ValidationAttribute
{
    private float _minPrice = minPrice;
    private float _maxPrice = maxPrice;

    /// <summary>
    /// Validates that the price is within the specified range.
    /// </summary>
    /// <param name="value">The price value to validate.</param>
    /// <returns>True if the price is within range; otherwise, false.</returns>
    public override bool IsValid(object? value)
    {
        if (value is float price)
        {
            return price >= _minPrice && price <= _maxPrice;
        }
        return false;
    }
}