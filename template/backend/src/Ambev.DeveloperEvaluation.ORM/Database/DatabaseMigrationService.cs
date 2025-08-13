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
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();

                _logger.LogInformation("Verificando migrations pendentes...");

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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar migrations: {Message}", ex.Message);
                throw;
            }
        }
    }
}
