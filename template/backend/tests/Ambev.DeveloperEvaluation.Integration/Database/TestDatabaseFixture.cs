using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Application;
using Ambev.DeveloperEvaluation.Common.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using MediatR;
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

        // Database
        services.AddDbContext<DefaultContext>(options =>
        {
            options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
                   .EnableSensitiveDataLogging()
                   .EnableDetailedErrors()
                   .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning));
        });

        // Repositories
        services.AddScoped<DbContext>(provider => provider.GetRequiredService<DefaultContext>());
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISaleRepository, SaleRepository>();

        // Security
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

        // AutoMapper
        services.AddAutoMapper(typeof(ApplicationLayer).Assembly);

        // MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ApplicationLayer).Assembly);
        });

        // Validation
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // Logging
        services.AddLogging();

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
    public ValueTask DisposeAsync()
    {
        _context?.Dispose();
        (_serviceProvider as IDisposable)?.Dispose();
        return ValueTask.CompletedTask;
    }
}
