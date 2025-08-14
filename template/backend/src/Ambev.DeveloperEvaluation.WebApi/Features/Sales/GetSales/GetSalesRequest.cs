namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales;

/// <summary>
/// Represents a request to get a paginated list of sales.
/// </summary>
public class GetSalesRequest
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
}
