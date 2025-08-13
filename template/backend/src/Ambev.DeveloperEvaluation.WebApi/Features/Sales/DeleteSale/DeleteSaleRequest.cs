namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale;

/// <summary>
/// Represents a request to delete a sale by ID.
/// </summary>
public class DeleteSaleRequest
{
    /// <summary>
    /// Gets or sets the unique identifier of the sale to delete.
    /// </summary>
    public Guid Id { get; set; }
}
