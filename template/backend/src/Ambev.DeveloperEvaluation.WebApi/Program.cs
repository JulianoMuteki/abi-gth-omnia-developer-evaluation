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
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("Ambev.DeveloperEvaluation.ORM")
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
                
                // Get seeding configuration from appsettings.json
                var shouldRunSeeding = builder.Configuration.GetValue<bool>("RUN_SEEDING", false);
                
                if (shouldRunSeeding)
                {
                    Log.Information("RUN_SEEDING is enabled in appsettings.json, executing seeding...");
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
                    Log.Information("RUN_SEEDING is not enabled in appsettings.json, skipping seeding");
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
}
