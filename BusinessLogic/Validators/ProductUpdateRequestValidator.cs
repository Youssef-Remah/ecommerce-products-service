using BusinessLogic.DTOs;
using FluentValidation;

namespace BusinessLogic.Validators;

public class ProductUpdateRequestValidator : AbstractValidator<ProductUpdateRequest>
{
    public ProductUpdateRequestValidator()
    {
        RuleFor(p => p.ProductID)
            .NotEmpty().WithMessage("Product ID cannot be blank");

        RuleFor(p => p.ProductName)
            .NotEmpty().WithMessage("Product name cannot be blank");

        RuleFor(p => p.Category)
           .IsInEnum().WithMessage("Invalid product category");

        RuleFor(p => p.UnitPrice)
            .InclusiveBetween(0, double.MaxValue).WithMessage($"Unit price should be between 0 to {double.MaxValue}");

        RuleFor(p => p.QuantityInStock)
            .InclusiveBetween(0, int.MaxValue).WithMessage($"Quantity should be between 0 to {int.MaxValue}");
    }
}
