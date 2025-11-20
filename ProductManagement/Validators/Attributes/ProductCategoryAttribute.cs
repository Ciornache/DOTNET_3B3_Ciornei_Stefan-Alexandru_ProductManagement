using System.ComponentModel.DataAnnotations;
using ProductManagement.Features.Products;

namespace ProductManagement.Validators.Attributes;

/// <summary>
/// Validates that a product category is one of the allowed categories.
/// </summary>
/// <param name="allowedCategories">The array of allowed product categories.</param>
public class ProductCategoryAttribute(params ProductCategory[] allowedCategories) : ValidationAttribute
{
    private ProductCategory[] _allowedCategories = allowedCategories;
    
    /// <summary>
    /// Validates that the category is in the allowed categories list.
    /// </summary>
    /// <param name="value">The category value to validate.</param>
    /// <returns>True if the category is allowed; otherwise, false.</returns>
    public override bool IsValid(object? value)
    {
        if (value is ProductCategory category)
        {
            return _allowedCategories.Contains(category);
        }

        return false;
    }
}