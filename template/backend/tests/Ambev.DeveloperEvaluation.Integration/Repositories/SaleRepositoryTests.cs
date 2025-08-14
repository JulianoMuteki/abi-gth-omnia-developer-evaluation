using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Integration.Database;
using Ambev.DeveloperEvaluation.Integration.TestData;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Repositories;

/// <summary>
/// Integration tests for SaleRepository using Testcontainers
/// </summary>
public class SaleRepositoryTests : IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;
    private readonly ISaleRepository _saleRepository;
    private readonly DefaultContext _context;

    public SaleRepositoryTests(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
        _context = fixture.Context;
        _saleRepository = new SaleRepository(_context);
    }

    /// <summary>
    /// Clean up database after each test to ensure isolation
    /// </summary>
    private async Task CleanupDatabaseAsync()
    {
        await _fixture.CleanDatabaseAsync();
    }

    [Fact]
    public async Task Database_ShouldBeAccessible()
    {
        // Arrange
        await CleanupDatabaseAsync();
        
        // Act & Assert
        _context.Database.CanConnect().Should().BeTrue();
    }

    [Fact]
    public async Task Context_ShouldCreateSaleTable()
    {
        // Arrange
        await CleanupDatabaseAsync();
        
        // Act
        var sale = SaleTestData.CreateFakeSale();
        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();
        
        // Assert
        var savedSale = await _context.Sales.FirstOrDefaultAsync(s => s.Id == sale.Id);
        savedSale.Should().NotBeNull();
        savedSale!.SaleNumber.Should().Be(sale.SaleNumber);
    }

    [Fact]
    public async Task AddAsync_ShouldAddSaleToDatabase()
    {
        // Arrange
        await CleanupDatabaseAsync();
        var sale = SaleTestData.CreateFakeSale();
        
        // Act
        var result = await _saleRepository.AddAsync(sale);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.SaleNumber.Should().Be(sale.SaleNumber);
        
        var savedSale = await _context.Sales
            .FirstOrDefaultAsync(s => s.Id == result.Id);
        
        savedSale.Should().NotBeNull();
        savedSale!.SaleNumber.Should().Be(sale.SaleNumber);
    }

    [Fact]
    public async Task AddAsync_ShouldAddSaleWithItemsToDatabase()
    {
        // Arrange
        await CleanupDatabaseAsync();
        var sale = SaleTestData.CreateFakeSale();
        var item1 = SaleTestData.CreateFakeSaleItem(sale.Id);
        var item2 = SaleTestData.CreateFakeSaleItem(sale.Id);
        sale.AddItem(item1);
        sale.AddItem(item2);

        // Act
        var result = await _saleRepository.AddAsync(sale);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        
        var savedSale = await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == result.Id);
        
        savedSale.Should().NotBeNull();
        savedSale!.Items.Should().HaveCount(2);
        
        var savedItems = await _context.SaleItems
            .Where(i => i.SaleId == result.Id)
            .ToListAsync();
        
        savedItems.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnSaleWithItems()
    {
        // Arrange
        await CleanupDatabaseAsync();
        var sale = SaleTestData.CreateFakeSale();
        var item = SaleTestData.CreateFakeSaleItem(sale.Id);
        sale.AddItem(item);
        
        await _context.Sales.AddAsync(sale);
        await _context.SaveChangesAsync();

        // Act
        var result = await _saleRepository.GetByIdAsync(sale.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(sale.Id);
        result.SaleNumber.Should().Be(sale.SaleNumber);
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCancelledSaleWithItems()
    {
        // Arrange
        await CleanupDatabaseAsync();
        var sale = SaleTestData.CreateFakeCancelledSale();
        var item = SaleTestData.CreateFakeCancelledSaleItem(sale.Id);
        await _context.SaleItems.AddAsync(item);
        
        await _context.Sales.AddAsync(sale);
        await _context.SaveChangesAsync();

        // Act
        var result = await _saleRepository.GetByIdAsync(sale.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(sale.Id);
        result.Status.Should().Be(SaleStatus.Cancelled);
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ShouldReturnNull()
    {
        // Arrange
        await CleanupDatabaseAsync();
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _saleRepository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetBySaleNumberAsync_ShouldReturnSaleWithItems()
    {
        // Arrange
        await CleanupDatabaseAsync();
        var sale = SaleTestData.CreateFakeSale("SALE-1234", Guid.NewGuid(), Guid.NewGuid());
        var item = SaleTestData.CreateFakeSaleItem(sale.Id);
        sale.AddItem(item);
        
        await _context.Sales.AddAsync(sale);
        await _context.SaveChangesAsync();

        // Act
        var result = await _saleRepository.GetBySaleNumberAsync("SALE-1234");

        // Assert
        result.Should().NotBeNull();
        result!.SaleNumber.Should().Be("SALE-1234");
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetBySaleNumberAsync_WithNonExistentNumber_ShouldReturnNull()
    {
        // Arrange
        await CleanupDatabaseAsync();
        
        // Act
        var result = await _saleRepository.GetBySaleNumberAsync("NON-EXISTENT");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllSales()
    {
        // Arrange
        await CleanupDatabaseAsync();
        var sales = SaleTestData.CreateFakeSales(3);
        foreach (var sale in sales)
        {
            var item = SaleTestData.CreateFakeSaleItem(sale.Id);
            sale.AddItem(item);
        }
        
        await _context.Sales.AddRangeAsync(sales);
        await _context.SaveChangesAsync();

        // Act
        var result = await _saleRepository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().AllSatisfy(sale => sale.Items.Should().NotBeEmpty());
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateSaleInDatabase()
    {
        // Arrange
        await CleanupDatabaseAsync();
        var sale = SaleTestData.CreateFakeSale();
        await _context.Sales.AddAsync(sale);
        await _context.SaveChangesAsync();

        // Act
        sale.CustomerName = "Updated Customer Name";
        sale.TotalAmount = 999.99m;
        var result = await _saleRepository.UpdateAsync(sale);

        // Assert
        result.Should().NotBeNull();
        result.CustomerName.Should().Be("Updated Customer Name");
        result.TotalAmount.Should().Be(999.99m);

        // Verify in database
        var updatedSale = await _context.Sales.FindAsync(sale.Id);
        updatedSale.Should().NotBeNull();
        updatedSale!.CustomerName.Should().Be("Updated Customer Name");
    }

    [Fact]
    public async Task DeleteAsync_WithSaleObject_ShouldDeleteSaleFromDatabase()
    {
        // Arrange
        await CleanupDatabaseAsync();
        var sale = SaleTestData.CreateFakeSale();
        await _context.Sales.AddAsync(sale);
        await _context.SaveChangesAsync();

        // Act
        var result = await _saleRepository.DeleteAsync(sale);

        // Assert
        result.Should().BeTrue();

        // Verify in database
        var deletedSale = await _context.Sales.FindAsync(sale.Id);
        deletedSale.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithId_ShouldDeleteSaleFromDatabase()
    {
        // Arrange
        await CleanupDatabaseAsync();
        var sale = SaleTestData.CreateFakeSale();
        await _context.Sales.AddAsync(sale);
        await _context.SaveChangesAsync();

        // Act
        var result = await _saleRepository.DeleteAsync(sale.Id);

        // Assert
        result.Should().BeTrue();

        // Verify in database
        var deletedSale = await _context.Sales.FindAsync(sale.Id);
        deletedSale.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_ShouldReturnFalse()
    {
        // Arrange
        await CleanupDatabaseAsync();
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _saleRepository.DeleteAsync(nonExistentId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task SaleNumberExistsAsync_WithExistingNumber_ShouldReturnTrue()
    {
        // Arrange
        await CleanupDatabaseAsync();
        var sale = SaleTestData.CreateFakeSale("SALE-5678", Guid.NewGuid(), Guid.NewGuid());
        await _context.Sales.AddAsync(sale);
        await _context.SaveChangesAsync();

        // Act
        var result = await _saleRepository.SaleNumberExistsAsync("SALE-5678");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task SaleNumberExistsAsync_WithNonExistentNumber_ShouldReturnFalse()
    {
        // Arrange
        await CleanupDatabaseAsync();
        
        // Act
        var result = await _saleRepository.SaleNumberExistsAsync("NON-EXISTENT");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetByCustomerIdAsync_ShouldReturnSalesForCustomer()
    {
        // Arrange
        await CleanupDatabaseAsync();
        var customerId = Guid.NewGuid();
        var sale1 = SaleTestData.CreateFakeSale("SALE-1001", customerId, Guid.NewGuid());
        var sale2 = SaleTestData.CreateFakeSale("SALE-1002", customerId, Guid.NewGuid());
        var sale3 = SaleTestData.CreateFakeSale("SALE-1003", Guid.NewGuid(), Guid.NewGuid()); // Different customer
        
        await _context.Sales.AddRangeAsync(sale1, sale2, sale3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _saleRepository.GetByCustomerIdAsync(customerId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(sale => sale.CustomerId.Should().Be(customerId));
    }

    [Fact]
    public async Task GetByBranchIdAsync_ShouldReturnSalesForBranch()
    {
        // Arrange
        await CleanupDatabaseAsync();
        var branchId = Guid.NewGuid();
        var sale1 = SaleTestData.CreateFakeSale("SALE-2001", Guid.NewGuid(), branchId);
        var sale2 = SaleTestData.CreateFakeSale("SALE-2002", Guid.NewGuid(), branchId);
        var sale3 = SaleTestData.CreateFakeSale("SALE-2003", Guid.NewGuid(), Guid.NewGuid()); // Different branch
        
        await _context.Sales.AddRangeAsync(sale1, sale2, sale3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _saleRepository.GetByBranchIdAsync(branchId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(sale => sale.BranchId.Should().Be(branchId));
    }

    [Fact]
    public async Task GetByDateRangeAsync_ShouldReturnSalesInDateRange()
    {
        // Arrange
        await CleanupDatabaseAsync();
        var startDate = DateTime.Today.AddDays(-10);
        var endDate = DateTime.Today.AddDays(-5);
        
        var sale1 = SaleTestData.CreateFakeSale();
        sale1.SaleDate = DateTime.Today.AddDays(-7);
        
        var sale2 = SaleTestData.CreateFakeSale();
        sale2.SaleDate = DateTime.Today.AddDays(-3);
        
        var sale3 = SaleTestData.CreateFakeSale();
        sale3.SaleDate = DateTime.Today.AddDays(-12);
        
        await _context.Sales.AddRangeAsync(sale1, sale2, sale3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _saleRepository.GetByDateRangeAsync(startDate, endDate);

        // Assert
        result.Should().HaveCount(1);
        result.Should().AllSatisfy(sale => 
            sale.SaleDate.Should().BeOnOrAfter(startDate).And.BeOnOrBefore(endDate));
    }

    [Fact]
    public async Task GetPaginatedAsync_ShouldReturnPaginatedResults()
    {
        // Arrange
        await CleanupDatabaseAsync();
        var sales = SaleTestData.CreateFakeSales(10);
        await _context.Sales.AddRangeAsync(sales);
        await _context.SaveChangesAsync();

        // Act
        var (result, totalCount) = await _saleRepository.GetPaginatedAsync(
            pageNumber: 1, 
            pageSize: 3);

        // Assert
        result.Should().HaveCount(3);
        totalCount.Should().Be(10);
    }

    [Fact]
    public async Task GetPaginatedAsync_WithFilters_ShouldReturnFilteredResults()
    {
        // Arrange
        await CleanupDatabaseAsync();
        var customerId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        
        var sale1 = SaleTestData.CreateFakeSale("SALE-3001", customerId, branchId, SaleStatus.Active);
        var sale2 = SaleTestData.CreateFakeSale("SALE-3002", customerId, Guid.NewGuid(), SaleStatus.Active);
        var sale3 = SaleTestData.CreateFakeSale("SALE-3003", Guid.NewGuid(), branchId, SaleStatus.Cancelled);
        
        await _context.Sales.AddRangeAsync(sale1, sale2, sale3);
        await _context.SaveChangesAsync();

        // Act
        var (result, totalCount) = await _saleRepository.GetPaginatedAsync(
            pageNumber: 1,
            pageSize: 10,
            customerId: customerId,
            branchId: branchId,
            status: "Active");

        // Assert
        result.Should().HaveCount(1);
        totalCount.Should().Be(1);
        result.Should().AllSatisfy(sale => 
        {
            sale.CustomerId.Should().Be(customerId);
            sale.BranchId.Should().Be(branchId);
            sale.Status.Should().Be(SaleStatus.Active);
        });
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldReturnNumberOfAffectedEntries()
    {
        // Arrange
        await CleanupDatabaseAsync();
        var sale = SaleTestData.CreateFakeSale();
        await _context.Sales.AddAsync(sale);

        // Act
        var result = await _saleRepository.SaveChangesAsync();

        // Assert
        result.Should().Be(1);
    }
}
