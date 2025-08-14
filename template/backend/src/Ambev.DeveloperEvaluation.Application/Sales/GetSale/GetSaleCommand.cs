using Ambev.DeveloperEvaluation.Common.Validation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Command for getting a sale by ID.
/// </summary>
/// <remarks>
/// This command is used to retrieve a sale by its unique identifier.
/// It implements <see cref="IRequest{TResponse}"/> to initiate the request 
/// that returns a <see cref="GetSaleResult"/>.
/// 
/// The data provided in this command is validated using the 
/// <see cref="GetSaleCommandValidator"/> which extends 
/// <see cref="AbstractValidator{T}"/> to ensure that the ID is correctly 
/// populated and follows the required rules.
/// </remarks>
public class GetSaleCommand : IRequest<GetSaleResult>
{
    /// <summary>
    /// Gets or sets the unique identifier of the sale to retrieve.
    /// </summary>
    public Guid Id { get; set; }

    public ValidationResultDetail Validate()
    {
        var validator = new GetSaleCommandValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}
