using Ambev.DeveloperEvaluation.Application;
using Ambev.DeveloperEvaluation.Common.HealthChecks;
using Ambev.DeveloperEvaluation.Common.Logging;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.IoC;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Database;
using Ambev.DeveloperEvaluation.WebApi.Middleware;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Ambev.DeveloperEvaluation.WebApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            Log.Information("Starting web application");

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.AddDefaultLogging();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.AddBasicHealthChecks();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<DefaultContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection") + ";Timeout=30;CommandTimeout=60;",
                    b => b.MigrationsAssembly("Ambev.DeveloperEvaluation.ORM")
                        .EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorCodesToAdd: null)
                        .CommandTimeout(60)
                )
            );

            builder.Services.AddJwtAuthentication(builder.Configuration);

            builder.RegisterDependencies();

            builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(ApplicationLayer).Assembly);

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(
                    typeof(ApplicationLayer).Assembly,
                    typeof(Program).Assembly
                );
            });

            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            var app = builder.Build();
            
            Log.Information("Application built successfully!");
            
            // Wait for database to be ready
            await WaitForDatabaseAsync(app.Services, builder.Configuration);
            
            try
            {
                Log.Information("Starting migration execution...");
                using (var scope = app.Services.CreateScope())
                {
                    Log.Information("Scope created, getting migration service...");
                    var migrationService = scope.ServiceProvider.GetRequiredService<IDatabaseMigrationService>();
                    Log.Information("Migration service obtained, executing...");
                    await migrationService.MigrateAsync();
                }
                Log.Information("Migrations executed successfully!");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error executing migrations: {Message}", ex.Message);
                Log.Error(ex, "Stack trace: {StackTrace}", ex.StackTrace);
            }

            // Database Seeding
            try
            {
                Log.Information("Checking if database seeding should be executed...");

                var shouldRunSeeding = false;
                
                var envRunSeeding = Environment.GetEnvironmentVariable("RUN_SEEDING");
                if (bool.TryParse(envRunSeeding, out var envSeeding))
                {
                    shouldRunSeeding = envSeeding;
                }
                
                Log.Information("RUN_SEEDING configuration: {shouldRunSeeding}", shouldRunSeeding);
                
                if (shouldRunSeeding)
                {
                    Log.Information("RUN_SEEDING is enabled, executing seeding...");
                    using (var scope = app.Services.CreateScope())
                    {
                        Log.Information("Scope created, getting seeding service...");
                        var seedingService = scope.ServiceProvider.GetRequiredService<IDatabaseSeedingService>();
                        Log.Information("Seeding service obtained, executing...");
                        await seedingService.SeedAsync();
                    }
                    Log.Information("Database seeding executed successfully!");
                }
                else
                {
                    Log.Information("RUN_SEEDING is not enabled, skipping seeding");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error executing database seeding: {Message}", ex.Message);
                Log.Error(ex, "Stack trace: {StackTrace}", ex.StackTrace);
            }
            
            Log.Information("Configuring middleware...");
            app.UseMiddleware<ValidationExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseBasicHealthChecks();

            app.MapControllers();

            Log.Information("Application fully configured!");
            Log.Information("Starting application...");
                      
            try
            {
                Log.Information("About to call app.Run()...");
                app.Run();
                Log.Information("app.Run() completed successfully!");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Error during app.Run(): {Message}", ex.Message);
                Log.Fatal(ex, "Stack trace: {StackTrace}", ex.StackTrace);
                throw;
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static async Task WaitForDatabaseAsync(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        const int maxRetries = 30;
        const int retryDelayMs = 2000;
        
        Log.Information("Waiting for database to be ready...");
        
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();
                
                Log.Information("Database connection attempt {Attempt}/{MaxRetries}", attempt, maxRetries);
                
                var canConnect = await context.Database.CanConnectAsync();
                if (canConnect)
                {
                    Log.Information("Database is ready!");
                    return;
                }
                
                Log.Warning("Database not ready yet, attempt {Attempt}/{MaxRetries}", attempt, maxRetries);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Database connection attempt {Attempt}/{MaxRetries} failed: {Message}", attempt, maxRetries, ex.Message);
            }
            
            if (attempt < maxRetries)
            {
                Log.Information("Waiting {Delay}ms before next attempt...", retryDelayMs);
                await Task.Delay(retryDelayMs);
            }
        }
        
        throw new InvalidOperationException($"Database is not ready after {maxRetries} attempts");
    }
}
