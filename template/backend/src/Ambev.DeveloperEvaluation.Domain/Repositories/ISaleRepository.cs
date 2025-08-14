using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Specifications;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Repository interface for Sale entity operations.
/// </summary>
public interface ISaleRepository
{
    /// <summary>
    /// Gets a sale by its ID.
    /// </summary>
    /// <param name="id">The sale ID.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale if found; otherwise, null.</returns>
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a sale by its sale number.
    /// </summary>
    /// <param name="saleNumber">The sale number.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale if found; otherwise, null.</returns>
    Task<Sale?> GetBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all sales that match the specified specification.
    /// </summary>
    /// <param name="specification">The specification to filter sales.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of sales that match the specification.</returns>
    Task<IEnumerable<Sale>> GetAsync(ISpecification<Sale> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all sales.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of all sales.</returns>
    Task<IEnumerable<Sale>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new sale to the repository.
    /// </summary>
    /// <param name="sale">The sale to add.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The added sale.</returns>
    Task<Sale> AddAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing sale in the repository.
    /// </summary>
    /// <param name="sale">The sale to update.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated sale.</returns>
    Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a sale from the repository.
    /// </summary>
    /// <param name="sale">The sale to delete.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the sale was deleted; otherwise, false.</returns>
    Task<bool> DeleteAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a sale from the repository by ID.
    /// </summary>
    /// <param name="id">The ID of the sale to delete.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the sale was deleted; otherwise, false.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves changes to the repository.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The number of affected entries.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a sale number already exists.
    /// </summary>
    /// <param name="saleNumber">The sale number to check.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the sale number exists; otherwise, false.</returns>
    Task<bool> SaleNumberExistsAsync(string saleNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets sales by customer ID.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of sales for the customer.</returns>
    Task<IEnumerable<Sale>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets sales by branch ID.
    /// </summary>
    /// <param name="branchId">The branch ID.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of sales for the branch.</returns>
    Task<IEnumerable<Sale>> GetByBranchIdAsync(Guid branchId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets sales within a date range.
    /// </summary>
    /// <param name="startDate">The start date.</param>
    /// <param name="endDate">The end date.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of sales within the date range.</returns>
    Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paginated list of sales with optional filters.
    /// </summary>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="customerId">Optional customer ID filter.</param>
    /// <param name="branchId">Optional branch ID filter.</param>
    /// <param name="startDate">Optional start date filter.</param>
    /// <param name="endDate">Optional end date filter.</param>
    /// <param name="status">Optional status filter.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A tuple containing the paginated sales and total count.</returns>
    Task<(IEnumerable<Sale> Sales, int TotalCount)> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        Guid? customerId = null,
        Guid? branchId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? status = null,
        CancellationToken cancellationToken = default);
}
