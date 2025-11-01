using System.ComponentModel.DataAnnotations;
using ProductManagement.Features.Products;

namespace ProductManagement.Validators.Attributes;

public class ProductCategoryAttribute(List<ProductCategory> allowedCategories) : ValidationAttribute
{
    private List<ProductCategory> _allowedCategories = allowedCategories;
    public bool IsValid(object? value)
    {
        if (value is ProductCategory category)
        {
            return _allowedCategories.Contains(category);
        }

        return false;
    }
}