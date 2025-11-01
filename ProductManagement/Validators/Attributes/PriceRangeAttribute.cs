using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Validators.Attributes;

public class PriceRangeAttribute(float minPrice, float maxPrice) : ValidationAttribute
{
    private float _minPrice = minPrice;
    private float _maxPrice = maxPrice;

    public bool IsValid(object? value)
    {
        if (value is float price)
        {
            return price >= _minPrice && price <= _maxPrice;
        }

        return false;
    }
}