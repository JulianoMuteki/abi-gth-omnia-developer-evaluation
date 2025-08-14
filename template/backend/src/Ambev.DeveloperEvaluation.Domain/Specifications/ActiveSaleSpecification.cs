using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Specifications;

/// <summary>
/// Specification to check if a sale is active.
/// A sale is considered active when its status is Active.
/// </summary>
public class ActiveSaleSpecification : ISpecification<Sale>
{
    /// <summary>
    /// Determines if the given sale satisfies the active sale criteria.
    /// </summary>
    /// <param name="sale">The sale to evaluate.</param>
    /// <returns>True if the sale is active; otherwise, false.</returns>
    public bool IsSatisfiedBy(Sale sale)
    {
        return sale.Status == SaleStatus.Active;
    }
}
