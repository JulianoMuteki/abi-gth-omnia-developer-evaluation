using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.ORM.Database
{
    public static class DatabaseMigrationExtension
    {
        public static IServiceCollection AddDatabaseMigrationService(this IServiceCollection services)
        {
            services.AddScoped<IDatabaseMigrationService, DatabaseMigrationService>();
            return services;
        }
    }
}
