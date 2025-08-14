namespace Ambev.DeveloperEvaluation.ORM.Database;

/// <summary>
/// Interface for database seeding service.
/// </summary>
public interface IDatabaseSeedingService
{
    /// <summary>
    /// Seeds the database with initial data if no data exists.
    /// </summary>
    Task SeedAsync();
}
