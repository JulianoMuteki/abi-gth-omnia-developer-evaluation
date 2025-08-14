using Ambev.DeveloperEvaluation.Common.Validation;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

/// <summary>
/// Validator for DeleteSaleCommand
/// </summary>
public class DeleteSaleCommandValidator : AbstractValidator<DeleteSaleCommand>
{
    /// <summary>
    /// Initializes a new instance of DeleteSaleCommandValidator
    /// </summary>
    public DeleteSaleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Sale ID is required");
    }
}
