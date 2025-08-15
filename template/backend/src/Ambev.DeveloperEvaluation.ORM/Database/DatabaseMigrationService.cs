using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.ORM.Database
{
    public interface IDatabaseMigrationService
    {
        Task MigrateAsync();
    }

    public class DatabaseMigrationService : IDatabaseMigrationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseMigrationService> _logger;

        public DatabaseMigrationService(IServiceProvider serviceProvider, ILogger<DatabaseMigrationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task MigrateAsync()
        {
            const int maxRetries = 5;
            const int retryDelayMs = 2000;
            
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    _logger.LogInformation("Migration attempt {Attempt}/{MaxRetries}", attempt, maxRetries);
                    
                    using var scope = _serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();

                    _logger.LogInformation("Verificando migrations pendentes...");

                    // Test database connection first
                    _logger.LogInformation("Testing database connection...");
                    var canConnect = await context.Database.CanConnectAsync();
                    if (!canConnect)
                    {
                        throw new InvalidOperationException("Cannot connect to database");
                    }
                    _logger.LogInformation("Database connection successful");

                    // Verifica se há migrations pendentes
                    var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                    var pendingMigrationsList = pendingMigrations.ToList();

                    if (!pendingMigrationsList.Any())
                    {
                        _logger.LogInformation("Nenhuma migration pendente encontrada. O banco de dados está atualizado.");
                        return;
                    }

                    _logger.LogInformation("Encontradas {Count} migrations pendentes: {Migrations}", 
                        pendingMigrationsList.Count, string.Join(", ", pendingMigrationsList));

                    // Executa as migrations
                    await context.Database.MigrateAsync();

                    _logger.LogInformation("Migrations aplicadas com sucesso!");
                    return; // Success, exit the retry loop
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Migration attempt {Attempt} failed: {Message}", attempt, ex.Message);
                    
                    if (attempt == maxRetries)
                    {
                        _logger.LogError(ex, "All migration attempts failed. Final error: {Message}", ex.Message);
                        throw;
                    }
                    
                    _logger.LogInformation("Waiting {Delay}ms before retry...", retryDelayMs);
                    await Task.Delay(retryDelayMs);
                }
            }
        }
    }
}
