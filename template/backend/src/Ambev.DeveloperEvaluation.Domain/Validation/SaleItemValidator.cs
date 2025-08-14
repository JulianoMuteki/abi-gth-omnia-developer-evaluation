using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

/// <summary>
/// Validator for the SaleItem entity.
/// Validates all sale item properties including product information,
/// quantity, pricing, and business rules.
/// </summary>
public class SaleItemValidator : AbstractValidator<SaleItem>
{
    /// <summary>
    /// Initializes a new instance of SaleItemValidator with validation rules.
    /// </summary>
    public SaleItemValidator()
    {
        RuleFor(item => item.SaleId)
            .NotEmpty()
            .WithMessage("Sale ID is required.");

        RuleFor(item => item.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required.");

        RuleFor(item => item.ProductName)
            .NotEmpty()
            .WithMessage("Product name is required.")
            .MaximumLength(100)
            .WithMessage("Product name cannot exceed 100 characters.");

        RuleFor(item => item.ProductCode)
            .NotEmpty()
            .WithMessage("Product code is required.")
            .Matches(@"^PROD-\d{4}$")
            .WithMessage("Product code must follow the format PROD-XXXX.")
            .MaximumLength(50)
            .WithMessage("Product code cannot exceed 50 characters.");

        RuleFor(item => item.ProductDescription)
            .MaximumLength(500)
            .WithMessage("Product description cannot exceed 500 characters.");

        RuleFor(item => item.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.")
            .LessThanOrEqualTo(20)
            .WithMessage("Quantity cannot exceed 20.");

        RuleFor(item => item.UnitPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Unit price cannot be negative.");

        RuleFor(item => item.DiscountPercentage)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Discount percentage cannot be negative.")
            .LessThanOrEqualTo(100)
            .WithMessage("Discount percentage cannot exceed 100%.");

        RuleFor(item => item.DiscountAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Discount amount cannot be negative.");

        RuleFor(item => item.TotalItemAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Total item amount cannot be negative.");

        RuleFor(item => item.Status)
            .NotEqual(SaleItemStatus.Unknown)
            .WithMessage("Sale item status cannot be Unknown.");

        // Business rule: Total amount should equal (UnitPrice * Quantity) - DiscountAmount
        RuleFor(item => item.TotalItemAmount)
            .Must((item, totalAmount) => 
            {
                var expectedTotal = (item.UnitPrice * item.Quantity) - item.DiscountAmount;
                return Math.Abs(totalAmount - expectedTotal) < 0.01m; // Allow for small decimal precision differences
            })
            .WithMessage("Total item amount must equal (UnitPrice * Quantity) - DiscountAmount.");

        // Business rule: Discount amount should equal (UnitPrice * Quantity * DiscountPercentage) / 100
        RuleFor(item => item.DiscountAmount)
            .Must((item, discountAmount) => 
            {
                var expectedDiscount = (item.UnitPrice * item.Quantity * item.DiscountPercentage) / 100;
                return Math.Abs(discountAmount - expectedDiscount) < 0.01m; // Allow for small decimal precision differences
            })
            .WithMessage("Discount amount must equal (UnitPrice * Quantity * DiscountPercentage) / 100.");
    }
}
