using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

/// <summary>
/// Validator for the Sale entity.
/// Validates all sale properties including sale number, customer information,
/// branch information, and business rules.
/// </summary>
public class SaleValidator : AbstractValidator<Sale>
{
    /// <summary>
    /// Initializes a new instance of SaleValidator with validation rules.
    /// </summary>
    public SaleValidator()
    {
        RuleFor(sale => sale.SaleNumber)
            .NotEmpty()
            .WithMessage("Sale number is required.")
            .MaximumLength(50)
            .WithMessage("Sale number cannot exceed 50 characters.")
            .Matches(@"^SALE-\d{5}$")
            .WithMessage("Sale number must follow the format SALE-XXXXX.");

        RuleFor(sale => sale.SaleDate)
            .NotEmpty()
            .WithMessage("Sale date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Sale date cannot be in the future.");

        RuleFor(sale => sale.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required.");

        RuleFor(sale => sale.CustomerName)
            .NotEmpty()
            .WithMessage("Customer name is required.")
            .MaximumLength(100)
            .WithMessage("Customer name cannot exceed 100 characters.");

        RuleFor(sale => sale.CustomerEmail)
            .NotEmpty()
            .WithMessage("Customer email is required.")
            .EmailAddress()
            .WithMessage("Customer email must be a valid email address.")
            .MaximumLength(100)
            .WithMessage("Customer email cannot exceed 100 characters.");

        RuleFor(sale => sale.CustomerPhone)
            .NotEmpty()
            .WithMessage("Customer phone is required.")
            .Matches(@"^\+55\d{10,11}$")
            .WithMessage("Customer phone must be a valid Brazilian phone number (+55XXXXXXXXXXX).")
            .MaximumLength(20)
            .WithMessage("Customer phone cannot exceed 20 characters.");

        RuleFor(sale => sale.BranchId)
            .NotEmpty()
            .WithMessage("Branch ID is required.");

        RuleFor(sale => sale.BranchName)
            .NotEmpty()
            .WithMessage("Branch name is required.")
            .MaximumLength(100)
            .WithMessage("Branch name cannot exceed 100 characters.");

        RuleFor(sale => sale.BranchCode)
            .NotEmpty()
            .WithMessage("Branch code is required.")
            .Matches(@"^BR-\d{3}$")
            .WithMessage("Branch code must follow the format BR-XXX.")
            .MaximumLength(20)
            .WithMessage("Branch code cannot exceed 20 characters.");

        RuleFor(sale => sale.TotalAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Total amount cannot be negative.");

        RuleFor(sale => sale.Status)
            .NotEqual(SaleStatus.Unknown)
            .WithMessage("Sale status cannot be Unknown.");

        RuleFor(sale => sale.Items)
            .NotEmpty()
            .WithMessage("Sale must have at least one item.");

        RuleForEach(sale => sale.Items)
            .SetValidator(new SaleItemValidator());
    }
}
