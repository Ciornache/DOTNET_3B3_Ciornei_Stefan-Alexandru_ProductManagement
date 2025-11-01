using FluentValidation;
using ProductManagement.Features.Products;

namespace ProductManagement.Validators;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Product name is required")
            .MaximumLength(100)
            .WithMessage("Product name cannot exceed 100 characters");

        RuleFor(x => x.Brand)
            .NotEmpty()
            .WithMessage("Brand is required")
            .MaximumLength(50)
            .WithMessage("Brand name cannot exceed 50 characters");

        RuleFor(x => x.SKU)
            .NotEmpty()
            .WithMessage("SKU is required")
            .Matches(@"^[A-Z0-9\-]+$")
            .WithMessage("SKU must contain only uppercase letters, numbers, and hyphens");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0");

        RuleFor(x => x.Category)
            .IsInEnum()
            .WithMessage("Invalid product category");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock quantity cannot be negative");

        RuleFor(x => x.ReleaseDate)
            .LessThanOrEqualTo(DateTime.Now.AddYears(1))
            .WithMessage("Release date cannot be more than 1 year in the future");
    }
}
