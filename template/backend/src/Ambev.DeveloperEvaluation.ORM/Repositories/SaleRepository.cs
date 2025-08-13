using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of ISaleRepository using Entity Framework Core
/// </summary>
public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    /// <summary>
    /// Initializes a new instance of SaleRepository
    /// </summary>
    /// <param name="context">The database context</param>
    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets a sale by its ID.
    /// </summary>
    /// <param name="id">The sale ID.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale if found; otherwise, null.</returns>
    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    /// <summary>
    /// Gets a sale by its sale number.
    /// </summary>
    /// <param name="saleNumber">The sale number.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale if found; otherwise, null.</returns>
    public async Task<Sale?> GetBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.SaleNumber == saleNumber, cancellationToken);
    }

    /// <summary>
    /// Gets all sales that match the specified specification.
    /// </summary>
    /// <param name="specification">The specification to filter sales.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of sales that match the specification.</returns>
    public async Task<IEnumerable<Sale>> GetAsync(ISpecification<Sale> specification, CancellationToken cancellationToken = default)
    {
        var allSales = await _context.Sales
            .Include(s => s.Items)
            .ToListAsync(cancellationToken);

        return allSales.Where(sale => specification.IsSatisfiedBy(sale));
    }

    /// <summary>
    /// Gets all sales.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of all sales.</returns>
    public async Task<IEnumerable<Sale>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Adds a new sale to the repository.
    /// </summary>
    /// <param name="sale">The sale to add.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The added sale.</returns>
    public async Task<Sale> AddAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        // Return the sale with items loaded
        return await GetByIdAsync(sale.Id, cancellationToken) ?? sale;
    }

    /// <summary>
    /// Updates an existing sale in the repository.
    /// </summary>
    /// <param name="sale">The sale to update.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated sale.</returns>
    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _context.Sales.Update(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    /// <summary>
    /// Deletes a sale from the repository.
    /// </summary>
    /// <param name="sale">The sale to delete.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the sale was deleted; otherwise, false.</returns>
    public async Task<bool> DeleteAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>
    /// Deletes a sale from the repository by ID.
    /// </summary>
    /// <param name="id">The ID of the sale to delete.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the sale was deleted; otherwise, false.</returns>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await GetByIdAsync(id, cancellationToken);
        if (sale == null)
            return false;

        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>
    /// Saves changes to the repository.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The number of affected entries.</returns>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Checks if a sale number already exists.
    /// </summary>
    /// <param name="saleNumber">The sale number to check.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the sale number exists; otherwise, false.</returns>
    public async Task<bool> SaleNumberExistsAsync(string saleNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .AnyAsync(s => s.SaleNumber == saleNumber, cancellationToken);
    }

    /// <summary>
    /// Gets sales by customer ID.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of sales for the customer.</returns>
    public async Task<IEnumerable<Sale>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Where(s => s.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets sales by branch ID.
    /// </summary>
    /// <param name="branchId">The branch ID.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of sales for the branch.</returns>
    public async Task<IEnumerable<Sale>> GetByBranchIdAsync(Guid branchId, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Where(s => s.BranchId == branchId)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets sales within a date range.
    /// </summary>
    /// <param name="startDate">The start date.</param>
    /// <param name="endDate">The end date.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of sales within the date range.</returns>
    public async Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
            .ToListAsync(cancellationToken);
    }
}
