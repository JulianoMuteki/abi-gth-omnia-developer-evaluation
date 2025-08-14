using Ambev.DeveloperEvaluation.Common.Validation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

/// <summary>
/// Command for deleting a sale by ID.
/// </summary>
/// <remarks>
/// This command is used to delete a sale by its unique identifier.
/// It implements <see cref="IRequest{TResponse}"/> to initiate the request 
/// that returns a <see cref="DeleteSaleResult"/>.
/// 
/// The data provided in this command is validated using the 
/// <see cref="DeleteSaleCommandValidator"/> which extends 
/// <see cref="AbstractValidator{T}"/> to ensure that the ID is correctly 
/// populated and follows the required rules.
/// </remarks>
public class DeleteSaleCommand : IRequest<DeleteSaleResult>
{
    /// <summary>
    /// Gets or sets the unique identifier of the sale to delete.
    /// </summary>
    public Guid Id { get; set; }

    public ValidationResultDetail Validate()
    {
        var validator = new DeleteSaleCommandValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}
