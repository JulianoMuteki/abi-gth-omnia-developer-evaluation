using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.EditSale;

/// <summary>
/// Validator for EditSaleRequest
/// </summary>
public class EditSaleRequestValidator : AbstractValidator<EditSaleRequest>
{
    /// <summary>
    /// Initializes a new instance of EditSaleRequestValidator
    /// </summary>
    public EditSaleRequestValidator()
    {
        RuleFor(x => x.SaleNumber)
            .NotEmpty()
            .WithMessage("Sale number is required")
            .MaximumLength(50)
            .WithMessage("Sale number cannot exceed 50 characters");

        RuleFor(x => x.SaleDate)
            .NotEmpty()
            .WithMessage("Sale date is required")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Sale date cannot be in the future");

        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required");

        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .WithMessage("Customer name is required")
            .MaximumLength(100)
            .WithMessage("Customer name cannot exceed 100 characters");

        RuleFor(x => x.CustomerEmail)
            .NotEmpty()
            .WithMessage("Customer email is required")
            .EmailAddress()
            .WithMessage("Customer email must be a valid email address")
            .MaximumLength(100)
            .WithMessage("Customer email cannot exceed 100 characters");

        RuleFor(x => x.CustomerPhone)
            .NotEmpty()
            .WithMessage("Customer phone is required")
            .MaximumLength(20)
            .WithMessage("Customer phone cannot exceed 20 characters");

        RuleFor(x => x.BranchId)
            .NotEmpty()
            .WithMessage("Branch ID is required");

        RuleFor(x => x.BranchName)
            .NotEmpty()
            .WithMessage("Branch name is required")
            .MaximumLength(100)
            .WithMessage("Branch name cannot exceed 100 characters");

        RuleFor(x => x.BranchCode)
            .NotEmpty()
            .WithMessage("Branch code is required")
            .MaximumLength(20)
            .WithMessage("Branch code cannot exceed 20 characters");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("At least one sale item is required");

        RuleForEach(x => x.Items)
            .SetValidator(new EditSaleItemRequestValidator());
    }
}

/// <summary>
/// Validator for EditSaleItemRequest
/// </summary>
public class EditSaleItemRequestValidator : AbstractValidator<EditSaleItemRequest>
{
    /// <summary>
    /// Initializes a new instance of EditSaleItemRequestValidator
    /// </summary>
    public EditSaleItemRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");

        RuleFor(x => x.ProductName)
            .NotEmpty()
            .WithMessage("Product name is required")
            .MaximumLength(100)
            .WithMessage("Product name cannot exceed 100 characters");

        RuleFor(x => x.ProductCode)
            .NotEmpty()
            .WithMessage("Product code is required")
            .MaximumLength(50)
            .WithMessage("Product code cannot exceed 50 characters");

        RuleFor(x => x.ProductDescription)
            .MaximumLength(500)
            .WithMessage("Product description cannot exceed 500 characters");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero")
            .LessThanOrEqualTo(20)
            .WithMessage("Quantity cannot exceed 20");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0)
            .WithMessage("Unit price must be greater than zero");

        RuleFor(x => x.DiscountPercentage)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Discount percentage cannot be negative")
            .LessThanOrEqualTo(100)
            .WithMessage("Discount percentage cannot exceed 100%");
    }
}
