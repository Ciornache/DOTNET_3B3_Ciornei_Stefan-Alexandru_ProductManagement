using ProductManagement.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Features.Products;

namespace ProductManagement.Validators;

/// <summary>
/// Validates product creation requests with comprehensive business rules including category-specific validations.
/// </summary>
public class CreateProductProfileValidator : AbstractValidator<CreateProductProfileCommand>
{
    private readonly ILogger<CreateProductProfileValidator> _logger;
    private readonly ProductManagementContext _productManagementContext;
    
    private readonly List<string> _blockedNames = new()
    {
        "Invalid",
        "Test",
        "Sample"
    };

    private readonly List<string> _inappropriateHomeWords = new()
    {
        "Weapon",
        "Gun",
        "Knife",
        "Bomb",
        "Adult",
        "Explicit",
        "Violence"
    };

    private readonly List<string> _technologyKeywords = new()
    {
        "Tech",
        "Gadget",
        "Device",
        "Electro",
        "Smart",
        "Digital",
        "Electronic",
        "Computer",
        "Mobile",
        "Wireless"
    };

    public CreateProductProfileValidator(ILogger<CreateProductProfileValidator> logger, ProductManagementContext productManagementContext)
    {
        _logger = logger;
        _productManagementContext = productManagementContext;

        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("Please specify a name")
            .MinimumLength(1).MaximumLength(200).WithMessage("The name must be between 1 and 200 characters")
            .Must(BeValidName).WithMessage("The specified name is not allowed")
            .MustAsync(BeUniqueName).WithMessage("The specified name already exists");
        
        RuleFor(command => command.Brand)
            .NotEmpty().WithMessage("Please specify a brand")
            .MinimumLength(2).MaximumLength(100)
            .Must(BeValidBrandName).WithMessage("The brand can only contain letters, digits, spaces, hyphens, ampersands, and dots");

        RuleFor(command => command.SKU)
            .NotEmpty().WithMessage("Please specify a SKU")
            .MinimumLength(5).MaximumLength(20)
            .Must(BeValidSKU).WithMessage("SKU must contain only uppercase letters, numbers, and hyphens")
            .MustAsync(BeUniqueSKU).WithMessage("The specified SKU already exists");
        
        RuleFor(command => command.Category)
            .IsInEnum().WithMessage("Please specify a valid category");
            
        RuleFor(command => command.Price)
            .GreaterThan(0).LessThan(10000).WithMessage("Please specify a valid price between 0 and 10,000");
            
        RuleFor(command => command.ReleaseDate)
            .LessThan(DateTime.Now).WithMessage("Release date must be in the past")
            .GreaterThan(DateTime.Now.AddYears(-10)).WithMessage("Release date cannot be more than 10 years ago")
            .Must(releaseDate => releaseDate.Year >= 1900).WithMessage("Please specify a valid release date (year must be 1900 or later)");
            
        RuleFor(command => command.StockQuantity)
            .GreaterThanOrEqualTo(0).LessThan(100_000).WithMessage("Please specify a valid stock quantity between 0 and 100,000");
            
        RuleFor(command => command.ImageUrl)
            .Must(BeValidImageUrl).When(command => !string.IsNullOrEmpty(command.ImageUrl))
            .WithMessage("Please specify a valid image URL");

        RuleFor(command => command)
            .Must(IsValidElectronicProduct)
            .When(command => command.Category == ProductCategory.Electronics)
            .WithMessage("Electronics product must meet specific requirements: price ≥ $50, contain technology keywords, and be released within 5 years");

        RuleFor(command => command.Price)
            .LessThanOrEqualTo(200m)
            .When(command => command.Category == ProductCategory.Home)
            .WithMessage("Home products cannot exceed $200.00");

        RuleFor(command => command.Name)
            .Must(BeAppropriateForHome)
            .When(command => command.Category == ProductCategory.Home)
            .WithMessage("Product name contains inappropriate words for home products");

        RuleFor(command => command.Brand)
            .MinimumLength(3)
            .When(command => command.Category == ProductCategory.Clothing)
            .WithMessage("Clothing brand name must be at least 3 characters");

        RuleFor(command => command.ImageUrl)
            .NotEmpty()
            .When(command => command.Category == ProductCategory.Clothing)
            .WithMessage("Clothing products must have an image URL");

        RuleFor(command => command.StockQuantity)
            .LessThanOrEqualTo(20)
            .When(command => command.Price > 100m)
            .WithMessage("Expensive products (>$100) must have limited stock (≤20 units)");

        RuleFor(command => command.StockQuantity)
            .LessThanOrEqualTo(50000)
            .When(command => command.Category == ProductCategory.Books)
            .WithMessage("Books products cannot have stock quantity over 50,000");

        RuleFor(command => command.SKU)
            .Must((command, sku) => sku.StartsWith("PREM-"))
            .When(command => command.Price > 500)
            .WithMessage("Premium products (>$500) must have SKU starting with 'PREM-'");
    }

    /// <summary>
    /// Validates that the product name is not in the blocked names list.
    /// </summary>
    /// <param name="name">The product name to validate.</param>
    /// <returns>True if the name is valid; otherwise, false.</returns>
    private bool BeValidName(string name)
    {
        _logger.LogInformation("Validating name: {Name}", name);
        return !_blockedNames.Contains(name);
    }

    /// <summary>
    /// Validates that the product name and brand combination is unique in the database.
    /// </summary>
    /// <param name="command">The product command containing name and brand.</param>
    /// <param name="name">The product name to check.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>True if the combination is unique; otherwise, false.</returns>
    private async Task<bool> BeUniqueName(CreateProductProfileCommand command, string name, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Checking uniqueness for Name: {Name} and Brand: {Brand}", name, command.Brand);
        
        var exists = await _productManagementContext.Products
            .AnyAsync(p => p.Name == name && p.Brand == command.Brand, cancellationToken);
            
        if (exists)
            _logger.LogWarning("Product with Name: {Name} and Brand: {Brand} already exists", name, command.Brand);
        
        return !exists;
    }

    /// <summary>
    /// Validates that the brand name contains only allowed characters (letters, digits, spaces, hyphens, ampersands, dots).
    /// </summary>
    /// <param name="brand">The brand name to validate.</param>
    /// <returns>True if the brand name is valid; otherwise, false.</returns>
    private bool BeValidBrandName(string brand)
    {
        _logger.LogInformation("Validating brand name: {Brand}", brand);
        return brand.All(ch => char.IsLetterOrDigit(ch) || char.IsWhiteSpace(ch) || ch == '-' || ch == '&' || ch == '.');
    }

    /// <summary>
    /// Validates that the SKU follows the correct format (uppercase letters, numbers, and hyphens).
    /// </summary>
    /// <param name="sku">The SKU to validate.</param>
    /// <returns>True if the SKU format is valid; otherwise, false.</returns>
    private bool BeValidSKU(string sku)
    {
        _logger.LogInformation("Validating SKU format: {SKU}", sku);
        return System.Text.RegularExpressions.Regex.IsMatch(sku, "^[A-Z0-9-]+$");
    }

    /// <summary>
    /// Validates that the SKU is unique in the database.
    /// </summary>
    /// <param name="sku">The SKU to check for uniqueness.</param>
    /// <param name="cancellationToken">Cancellation token for async operation.</param>
    /// <returns>True if the SKU is unique; otherwise, false.</returns>
    private async Task<bool> BeUniqueSKU(string sku, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Checking SKU uniqueness: {SKU}", sku);
        
        var exists = await _productManagementContext.Products
            .AnyAsync(p => p.SKU == sku, cancellationToken);
            
        if (exists)
        {
            _logger.LogWarning("SKU already exists: {SKU}", sku);
        }
        
        return !exists;
    }

    /// <summary>
    /// Validates that the image URL is properly formatted and has a valid image extension.
    /// </summary>
    /// <param name="imageUrl">The image URL to validate.</param>
    /// <returns>True if the URL is valid or empty; otherwise, false.</returns>
    private bool BeValidImageUrl(string? imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
            return true;

        _logger.LogInformation("Validating image URL: {ImageUrl}", imageUrl);
        
        var urlPattern = @"^(http|https)://[a-zA-Z0-9./_-]+\.(jpg|png|gif|bmp)$";
        return System.Text.RegularExpressions.Regex.IsMatch(imageUrl, urlPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    }
    
    /// <summary>
    /// Validates that an electronics product meets all business requirements (price, keywords, release date).
    /// </summary>
    /// <param name="command">The product command to validate.</param>
    /// <returns>True if all electronics requirements are met; otherwise, false.</returns>
    private bool IsValidElectronicProduct(CreateProductProfileCommand command)
    {
        _logger.LogInformation("Validating electronics product: {Name}", command.Name);
        
        if (command.Price < 50)
        {
            _logger.LogWarning("Electronics product {Name} has price below $50", command.Name);
            return false;
        }

        if (!ContainTechnologyKeywords(command.Name))
        {
            _logger.LogWarning("Electronics product {Name} does not contain technology keywords", command.Name);
            return false;
        }

        var fiveYearsAgo = DateTime.Now.AddYears(-5);
        if (command.ReleaseDate < fiveYearsAgo)
        {
            _logger.LogWarning("Electronics product {Name} is not recent (older than 5 years)", command.Name);
            return false;
        }

        return true;
    }
    
    /// <summary>
    /// Checks if the product name contains technology-related keywords.
    /// </summary>
    /// <param name="name">The product name to check.</param>
    /// <returns>True if technology keywords are found; otherwise, false.</returns>
    private bool ContainTechnologyKeywords(string name)
    {
        _logger.LogInformation("Checking technology keywords in name: {Name}", name);
        return _technologyKeywords.Any(keyword => name.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Validates that the product name does not contain inappropriate words for home products.
    /// </summary>
    /// <param name="name">The product name to check.</param>
    /// <returns>True if the name is appropriate for home products; otherwise, false.</returns>
    private bool BeAppropriateForHome(string name)
    {
        _logger.LogInformation("Checking home product appropriateness for name: {Name}", name);
        return !_inappropriateHomeWords.Any(word => name.Contains(word, StringComparison.OrdinalIgnoreCase));
    }
}