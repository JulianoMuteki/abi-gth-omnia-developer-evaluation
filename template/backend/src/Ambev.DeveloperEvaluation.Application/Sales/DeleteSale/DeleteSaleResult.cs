namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

/// <summary>
/// Represents the response returned after successfully deleting a sale.
/// </summary>
/// <remarks>
/// This response indicates that the sale was successfully deleted from the system.
/// </remarks>
public class DeleteSaleResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the deletion was successful.
    /// </summary>
    public bool Success { get; set; }
}
