using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Specifications;

/// <summary>
/// Specification to check if a sale item is active.
/// A sale item is considered active when its status is Active.
/// </summary>
public class ActiveSaleItemSpecification : ISpecification<SaleItem>
{
    /// <summary>
    /// Determines if the given sale item satisfies the active sale item criteria.
    /// </summary>
    /// <param name="saleItem">The sale item to evaluate.</param>
    /// <returns>True if the sale item is active; otherwise, false.</returns>
    public bool IsSatisfiedBy(SaleItem saleItem)
    {
        return saleItem.Status == SaleItemStatus.Active;
    }
}
