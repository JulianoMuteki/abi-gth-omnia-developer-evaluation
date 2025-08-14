using Ambev.DeveloperEvaluation.ORM;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Database;

/// <summary>
/// Fixture for database tests using In-Memory database
/// </summary>
public class TestDatabaseFixture : IAsyncDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly DefaultContext _context;

    public TestDatabaseFixture()
    {
        var services = new ServiceCollection();

        services.AddDbContext<DefaultContext>(options =>
        {
            options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
                   .EnableSensitiveDataLogging()
                   .EnableDetailedErrors()
                   .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning));
        });

        _serviceProvider = services.BuildServiceProvider();

        _context = _serviceProvider.GetRequiredService<DefaultContext>();
        _context.Database.EnsureCreated();
    }

    /// <summary>
    /// Gets the database context
    /// </summary>
    public DefaultContext Context => _context;

    /// <summary>
    /// Gets the service provider for dependency injection
    /// </summary>
    public IServiceProvider ServiceProvider => _serviceProvider;

    /// <summary>
    /// Cleans up the database by removing all data
    /// </summary>
    public async Task CleanDatabaseAsync()
    {
        _context.SaleItems.RemoveRange(_context.SaleItems);
        _context.Sales.RemoveRange(_context.Sales);
        _context.Branches.RemoveRange(_context.Branches);
        _context.Users.RemoveRange(_context.Users);
        
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Disposes the fixture
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        _context?.Dispose();
        (_serviceProvider as IDisposable)?.Dispose();
    }
}
