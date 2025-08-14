using Ambev.DeveloperEvaluation.Common.Validation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

/// <summary>
/// Command for getting a paginated list of sales.
/// </summary>
/// <remarks>
/// This command is used to retrieve a paginated list of sales.
/// It implements <see cref="IRequest{TResponse}"/> to initiate the request 
/// that returns a <see cref="GetSalesResult"/>.
/// 
/// The data provided in this command is validated using the 
/// <see cref="GetSalesCommandValidator"/> which extends 
/// <see cref="AbstractValidator{T}"/> to ensure that the pagination parameters 
/// are correctly populated and follow the required rules.
/// </remarks>
public class GetSalesCommand : IRequest<GetSalesResult>
{
    /// <summary>
    /// Gets or sets the page number (1-based).
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the page size.
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Gets or sets the optional customer ID filter.
    /// </summary>
    public Guid? CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the optional branch ID filter.
    /// </summary>
    public Guid? BranchId { get; set; }

    /// <summary>
    /// Gets or sets the optional start date filter.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Gets or sets the optional end date filter.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Gets or sets the optional sale status filter.
    /// </summary>
    public string? Status { get; set; }

    public ValidationResultDetail Validate()
    {
        var validator = new GetSalesCommandValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}
