using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ProductManagement.Validators.Attributes;

public class ValidSKUAttribute : ValidationAttribute, IClientModelValidator
{
    public override bool IsValid(object? value) {
        
        string sku = value as string ?? string.Empty;
        
        if(sku.Length < 5 || sku.Length > 20)
            return false;
        
        sku = sku.Replace(" ", "");
        return Regex.Match(sku, @"^[a-zA-Z0-9_]+$").Success;
    }
    
    public void AddValidation(ClientModelValidationContext context)
    {
        context.Attributes.Add("data-val", "true");
        context.Attributes.Add("data-val-validsku", "The SKU must be 5-20 characters long and contain only letters, numbers, and underscores.");
    }
}