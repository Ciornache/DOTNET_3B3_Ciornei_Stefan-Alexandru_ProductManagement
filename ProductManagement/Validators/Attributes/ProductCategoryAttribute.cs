using System.ComponentModel.DataAnnotations;
using ProductManagement.Features.Products;

namespace ProductManagement.Validators.Attributes;

public class ProductCategoryAttribute(params ProductCategory[] allowedCategories) : ValidationAttribute
{
    private ProductCategory[] _allowedCategories = allowedCategories;
    
    public override bool IsValid(object? value)
    {
        if (value is ProductCategory category)
        {
            return _allowedCategories.Contains(category);
        }

        return false;
    }
}