using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ProductManagement.Validators.Attributes;

/// <summary>
/// Validates that a SKU follows the required format: 5-20 characters containing only letters, numbers, and underscores.
/// </summary>
public class ValidSKUAttribute : ValidationAttribute, IClientModelValidator
{
    /// <summary>
    /// Validates the SKU format on the server side.
    /// </summary>
    /// <param name="value">The SKU value to validate.</param>
    /// <returns>True if the SKU is valid; otherwise, false.</returns>
    public override bool IsValid(object? value) {
        
        string sku = value as string ?? string.Empty;
        
        if(sku.Length < 5 || sku.Length > 20)
            return false;
        
        sku = sku.Replace(" ", "");
        return Regex.Match(sku, @"^[a-zA-Z0-9_]+$").Success;
    }
    
    /// <summary>
    /// Adds client-side validation attributes for SKU validation.
    /// </summary>
    /// <param name="context">The client model validation context.</param>
    public void AddValidation(ClientModelValidationContext context)
    {
        context.Attributes.Add("data-val", "true");
        context.Attributes.Add("data-val-validsku", "The SKU must be 5-20 characters long and contain only letters, numbers, and underscores.");
    }
}