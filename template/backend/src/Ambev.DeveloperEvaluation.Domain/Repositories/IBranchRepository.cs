using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Specifications;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Repository interface for Branch entity operations.
/// </summary>
public interface IBranchRepository
{
    /// <summary>
    /// Gets a branch by its ID.
    /// </summary>
    /// <param name="id">The branch ID.</param>
    /// <returns>The branch if found; otherwise, null.</returns>
    Task<Branch?> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets a branch by its code.
    /// </summary>
    /// <param name="code">The branch code.</param>
    /// <returns>The branch if found; otherwise, null.</returns>
    Task<Branch?> GetByCodeAsync(string code);

    /// <summary>
    /// Gets all branches that match the specified specification.
    /// </summary>
    /// <param name="specification">The specification to filter branches.</param>
    /// <returns>A collection of branches that match the specification.</returns>
    Task<IEnumerable<Branch>> GetAsync(ISpecification<Branch> specification);

    /// <summary>
    /// Gets all branches.
    /// </summary>
    /// <returns>A collection of all branches.</returns>
    Task<IEnumerable<Branch>> GetAllAsync();

    /// <summary>
    /// Gets all active branches.
    /// </summary>
    /// <returns>A collection of active branches.</returns>
    Task<IEnumerable<Branch>> GetActiveAsync();

    /// <summary>
    /// Adds a new branch to the repository.
    /// </summary>
    /// <param name="branch">The branch to add.</param>
    /// <returns>The added branch.</returns>
    Task<Branch> AddAsync(Branch branch);

    /// <summary>
    /// Updates an existing branch in the repository.
    /// </summary>
    /// <param name="branch">The branch to update.</param>
    /// <returns>The updated branch.</returns>
    Task<Branch> UpdateAsync(Branch branch);

    /// <summary>
    /// Deletes a branch from the repository.
    /// </summary>
    /// <param name="id">The ID of the branch to delete.</param>
    /// <returns>True if the branch was deleted; otherwise, false.</returns>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Checks if a branch code already exists.
    /// </summary>
    /// <param name="code">The branch code to check.</param>
    /// <returns>True if the branch code exists; otherwise, false.</returns>
    Task<bool> CodeExistsAsync(string code);
}
