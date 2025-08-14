using Ambev.DeveloperEvaluation.Common.Validation;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Validator for GetSaleCommand
/// </summary>
public class GetSaleCommandValidator : AbstractValidator<GetSaleCommand>
{
    /// <summary>
    /// Initializes a new instance of GetSaleCommandValidator
    /// </summary>
    public GetSaleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Sale ID is required");
    }
}
