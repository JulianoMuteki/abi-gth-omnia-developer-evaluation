using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

/// <summary>
/// Validator for GetSalesCommand
/// </summary>
public class GetSalesCommandValidator : AbstractValidator<GetSalesCommand>
{
    /// <summary>
    /// Initializes a new instance of GetSalesCommandValidator
    /// </summary>
    public GetSalesCommandValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("Start date must be less than or equal to end date");

        RuleFor(x => x.Status)
            .Must(BeValidSaleStatus)
            .When(x => !string.IsNullOrEmpty(x.Status))
            .WithMessage("Status must be a valid sale status (Active, Cancelled, Completed)");
    }

    private static bool BeValidSaleStatus(string? status)
    {
        if (string.IsNullOrEmpty(status))
            return true;

        return Enum.TryParse<SaleStatus>(status, true, out _);
    }
}
