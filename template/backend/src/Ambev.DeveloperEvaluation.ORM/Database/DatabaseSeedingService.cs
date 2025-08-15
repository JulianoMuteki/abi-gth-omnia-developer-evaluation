using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.ORM.Database;

/// <summary>
/// Service responsible for seeding initial data into the database.
/// </summary>
public class DatabaseSeedingService : IDatabaseSeedingService
{
    private readonly DefaultContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<DatabaseSeedingService> _logger;

    public DatabaseSeedingService(
        DefaultContext context,
        IPasswordHasher passwordHasher,
        ILogger<DatabaseSeedingService> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    /// <summary>
    /// Seeds the database with initial data if no data exists.
    /// </summary>
    public async Task SeedAsync()
    {
        try
        {
            _logger.LogInformation("Starting database seeding...");

            await SeedUsersAsync();
            await _context.SaveChangesAsync();
            
            await SeedBranchesAsync();
            await _context.SaveChangesAsync();
            
            await SeedSalesAsync();
            await _context.SaveChangesAsync();
            
            await SeedSaleItemsAsync();
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Database seeding completed successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during database seeding");
            throw;
        }
    }

    private async Task SeedUsersAsync()
    {
        if (await _context.Users.AnyAsync())
        {
            _logger.LogInformation("Users table already has data, skipping seeding");
            return;
        }

        _logger.LogInformation("Seeding Users table...");

        var users = new List<User>
        {
            new User
            {
                Username = "admin_user",
                Email = "admin@ambev.com",
                Phone = "(11) 99999-9999",
                Password = _passwordHasher.HashPassword("Admin@123"),
                Role = UserRole.Admin,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Username = "manager_user",
                Email = "manager@ambev.com",
                Phone = "(11) 88888-8888",
                Password = _passwordHasher.HashPassword("Manager@123"),
                Role = UserRole.Manager,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Username = "customer_user1",
                Email = "customer1@example.com",
                Phone = "(11) 77777-7777",
                Password = _passwordHasher.HashPassword("Customer@123"),
                Role = UserRole.Customer,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Username = "customer_user2",
                Email = "customer2@example.com",
                Phone = "(11) 66666-6666",
                Password = _passwordHasher.HashPassword("Customer@123"),
                Role = UserRole.Customer,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Username = "customer_user3",
                Email = "customer3@example.com",
                Phone = "(11) 55555-5555",
                Password = _passwordHasher.HashPassword("Customer@123"),
                Role = UserRole.Customer,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow
            }
        };

        await _context.Users.AddRangeAsync(users);
        _logger.LogInformation("Added {Count} users to the database", users.Count);
    }

    private async Task SeedBranchesAsync()
    {
        if (await _context.Branches.AnyAsync())
        {
            _logger.LogInformation("Branches table already has data, skipping seeding");
            return;
        }

        _logger.LogInformation("Seeding Branches table...");

        var branches = new List<Branch>
        {
            new Branch
            {
                Name = "Filial Centro",
                Code = "CENTRO001",
                Address = "Rua das Flores, 123 - Centro, São Paulo - SP",
                Phone = "(11) 3333-3333",
                Email = "centro@ambev.com",
                Status = BranchStatus.Active,
                CreatedAt = DateTime.UtcNow
            },
            new Branch
            {
                Name = "Filial Zona Sul",
                Code = "SUL002",
                Address = "Av. Paulista, 456 - Bela Vista, São Paulo - SP",
                Phone = "(11) 4444-4444",
                Email = "sul@ambev.com",
                Status = BranchStatus.Active,
                CreatedAt = DateTime.UtcNow
            },
            new Branch
            {
                Name = "Filial Zona Norte",
                Code = "NORTE003",
                Address = "Rua Augusta, 789 - Consolação, São Paulo - SP",
                Phone = "(11) 5555-5555",
                Email = "norte@ambev.com",
                Status = BranchStatus.Active,
                CreatedAt = DateTime.UtcNow
            },
            new Branch
            {
                Name = "Filial Zona Leste",
                Code = "LESTE004",
                Address = "Av. Radial Leste, 321 - Tatuapé, São Paulo - SP",
                Phone = "(11) 6666-6666",
                Email = "leste@ambev.com",
                Status = BranchStatus.Active,
                CreatedAt = DateTime.UtcNow
            },
            new Branch
            {
                Name = "Filial Zona Oeste",
                Code = "OESTE005",
                Address = "Rua Corifeu de Azevedo Marques, 654 - Butantã, São Paulo - SP",
                Phone = "(11) 7777-7777",
                Email = "oeste@ambev.com",
                Status = BranchStatus.Active,
                CreatedAt = DateTime.UtcNow
            }
        };

        await _context.Branches.AddRangeAsync(branches);
        _logger.LogInformation("Added {Count} branches to the database", branches.Count);
    }

    private async Task SeedSalesAsync()
    {
        if (await _context.Sales.AnyAsync())
        {
            _logger.LogInformation("Sales table already has data, skipping seeding");
            return;
        }

        _logger.LogInformation("Seeding Sales table...");

        var branches = await _context.Branches.ToListAsync();
        if (!branches.Any())
        {
            _logger.LogWarning("No branches found, skipping sales seeding");
            return;
        }

        var sales = new List<Sale>
        {
            new Sale
            {
                SaleNumber = "SALE001",
                SaleDate = DateTime.UtcNow.AddDays(-30),
                CustomerId = Guid.NewGuid(),
                CustomerName = "João Silva",
                CustomerEmail = "joao.silva@email.com",
                CustomerPhone = "(11) 11111-1111",
                BranchId = branches[0].Id,
                BranchName = branches[0].Name,
                BranchCode = branches[0].Code,
                TotalAmount = 150.00m,
                Status = SaleStatus.Active,
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new Sale
            {
                SaleNumber = "SALE002",
                SaleDate = DateTime.UtcNow.AddDays(-25),
                CustomerId = Guid.NewGuid(),
                CustomerName = "Maria Santos",
                CustomerEmail = "maria.santos@email.com",
                CustomerPhone = "(11) 22222-2222",
                BranchId = branches[1].Id,
                BranchName = branches[1].Name,
                BranchCode = branches[1].Code,
                TotalAmount = 89.50m,
                Status = SaleStatus.Active,
                CreatedAt = DateTime.UtcNow.AddDays(-25)
            },
            new Sale
            {
                SaleNumber = "SALE003",
                SaleDate = DateTime.UtcNow.AddDays(-20),
                CustomerId = Guid.NewGuid(),
                CustomerName = "Pedro Oliveira",
                CustomerEmail = "pedro.oliveira@email.com",
                CustomerPhone = "(11) 33333-3333",
                BranchId = branches[2].Id,
                BranchName = branches[2].Name,
                BranchCode = branches[2].Code,
                TotalAmount = 220.75m,
                Status = SaleStatus.Active,
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            },
            new Sale
            {
                SaleNumber = "SALE004",
                SaleDate = DateTime.UtcNow.AddDays(-15),
                CustomerId = Guid.NewGuid(),
                CustomerName = "Ana Costa",
                CustomerEmail = "ana.costa@email.com",
                CustomerPhone = "(11) 44444-4444",
                BranchId = branches[3].Id,
                BranchName = branches[3].Name,
                BranchCode = branches[3].Code,
                TotalAmount = 75.25m,
                Status = SaleStatus.Active,
                CreatedAt = DateTime.UtcNow.AddDays(-15)
            },
            new Sale
            {
                SaleNumber = "SALE005",
                SaleDate = DateTime.UtcNow.AddDays(-10),
                CustomerId = Guid.NewGuid(),
                CustomerName = "Carlos Ferreira",
                CustomerEmail = "carlos.ferreira@email.com",
                CustomerPhone = "(11) 55555-5555",
                BranchId = branches[4].Id,
                BranchName = branches[4].Name,
                BranchCode = branches[4].Code,
                TotalAmount = 180.00m,
                Status = SaleStatus.Active,
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            }
        };

        await _context.Sales.AddRangeAsync(sales);
        _logger.LogInformation("Added {Count} sales to the database", sales.Count);
    }

    private async Task SeedSaleItemsAsync()
    {
        if (await _context.SaleItems.AnyAsync())
        {
            _logger.LogInformation("SaleItems table already has data, skipping seeding");
            return;
        }

        _logger.LogInformation("Seeding SaleItems table...");

        var sales = await _context.Sales.ToListAsync();
        if (!sales.Any())
        {
            _logger.LogWarning("No sales found, skipping sale items seeding");
            return;
        }

        var saleItems = new List<SaleItem>
        {
            new SaleItem
            {
                SaleId = sales[0].Id,
                ProductId = Guid.NewGuid(),
                ProductName = "Cerveja Brahma",
                ProductCode = "BRAHMA001",
                ProductDescription = "Cerveja Brahma 350ml",
                Quantity = 2,
                UnitPrice = 75.00m,
                DiscountPercentage = 0.00m,
                DiscountAmount = 0.00m,
                TotalItemAmount = 150.00m,
                Status = SaleItemStatus.Active,
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new SaleItem
            {
                SaleId = sales[1].Id,
                ProductId = Guid.NewGuid(),
                ProductName = "Cerveja Skol",
                ProductCode = "SKOL001",
                ProductDescription = "Cerveja Skol 350ml",
                Quantity = 1,
                UnitPrice = 89.50m,
                DiscountPercentage = 0.00m,
                DiscountAmount = 0.00m,
                TotalItemAmount = 89.50m,
                Status = SaleItemStatus.Active,
                CreatedAt = DateTime.UtcNow.AddDays(-25)
            },
            new SaleItem
            {
                SaleId = sales[2].Id,
                ProductId = Guid.NewGuid(),
                ProductName = "Cerveja Antarctica",
                ProductCode = "ANTARCTICA001",
                ProductDescription = "Cerveja Antarctica 350ml",
                Quantity = 3,
                UnitPrice = 73.58m,
                DiscountPercentage = 0.00m,
                DiscountAmount = 0.00m,
                TotalItemAmount = 220.75m,
                Status = SaleItemStatus.Active,
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            },
            new SaleItem
            {
                SaleId = sales[3].Id,
                ProductId = Guid.NewGuid(),
                ProductName = "Refrigerante Guaraná",
                ProductCode = "GUARANA001",
                ProductDescription = "Refrigerante Guaraná 2L",
                Quantity = 1,
                UnitPrice = 75.25m,
                DiscountPercentage = 0.00m,
                DiscountAmount = 0.00m,
                TotalItemAmount = 75.25m,
                Status = SaleItemStatus.Active,
                CreatedAt = DateTime.UtcNow.AddDays(-15)
            },
            new SaleItem
            {
                SaleId = sales[4].Id,
                ProductId = Guid.NewGuid(),
                ProductName = "Cerveja Heineken",
                ProductCode = "HEINEKEN001",
                ProductDescription = "Cerveja Heineken 350ml",
                Quantity = 2,
                UnitPrice = 90.00m,
                DiscountPercentage = 0.00m,
                DiscountAmount = 0.00m,
                TotalItemAmount = 180.00m,
                Status = SaleItemStatus.Active,
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            }
        };

        await _context.SaleItems.AddRangeAsync(saleItems);
        _logger.LogInformation("Added {Count} sale items to the database", saleItems.Count);
        _logger.LogInformation("Sale items will be saved in the next SaveChanges call");
    }
}
