using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Bogus;

namespace Ambev.DeveloperEvaluation.Integration.TestData;

/// <summary>
/// Test data generator for Sale and SaleItem entities
/// </summary>
public static class SaleTestData
{
    /// <summary>
    /// Creates a fake sale with default values
    /// </summary>
    public static Sale CreateFakeSale()
    {
        var saleFaker = new Faker<Sale>()
            .RuleFor(s => s.Id, f => Guid.NewGuid())
            .RuleFor(s => s.SaleNumber, f => $"SALE-{f.Random.Number(1000, 9999)}")
            .RuleFor(s => s.SaleDate, f => f.Date.Recent(30))
            .RuleFor(s => s.CustomerId, f => f.Random.Guid())
            .RuleFor(s => s.CustomerName, f => f.Person.FullName)
            .RuleFor(s => s.CustomerEmail, f => f.Person.Email)
            .RuleFor(s => s.CustomerPhone, f => f.Phone.PhoneNumber())
            .RuleFor(s => s.BranchId, f => f.Random.Guid())
            .RuleFor(s => s.BranchName, f => f.Company.CompanyName())
            .RuleFor(s => s.BranchCode, f => f.Random.AlphaNumeric(6).ToUpper())
            .RuleFor(s => s.TotalAmount, f => f.Random.Decimal(10, 1000))
            .RuleFor(s => s.Status, SaleStatus.Active)
            .RuleFor(s => s.CreatedAt, f => f.Date.Recent(30))
            .RuleFor(s => s.UpdatedAt, f => f.Date.Recent(30))
            .RuleFor(s => s.CancelledAt, (f, s) => s.Status == SaleStatus.Cancelled ? f.Date.Recent(30) : null)
            .RuleFor(s => s.CancelledBy, (f, s) => s.Status == SaleStatus.Cancelled ? f.Random.Guid() : null);

        return saleFaker.Generate();
    }

    /// <summary>
    /// Creates a fake sale with specific values
    /// </summary>
    public static Sale CreateFakeSale(
        string saleNumber,
        Guid customerId,
        Guid branchId,
        SaleStatus status = SaleStatus.Active)
    {
        var sale = CreateFakeSale();
        sale.SaleNumber = saleNumber;
        sale.CustomerId = customerId;
        sale.BranchId = branchId;
        sale.Status = status;
        return sale;
    }

    /// <summary>
    /// Creates a fake sale item with default values
    /// </summary>
    public static SaleItem CreateFakeSaleItem(Guid saleId)
    {
        var itemFaker = new Faker<SaleItem>()
            .RuleFor(i => i.Id, f => Guid.NewGuid())
            .RuleFor(i => i.SaleId, saleId)
            .RuleFor(i => i.ProductId, f => f.Random.Guid())
            .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
            .RuleFor(i => i.ProductCode, f => f.Random.AlphaNumeric(8).ToUpper())
            .RuleFor(i => i.ProductDescription, f => f.Commerce.ProductDescription())
            .RuleFor(i => i.Quantity, f => f.Random.Int(1, 20))
            .RuleFor(i => i.UnitPrice, f => f.Random.Decimal(5, 100))
            .RuleFor(i => i.DiscountPercentage, f => f.Random.Decimal(0, 20))
            .RuleFor(i => i.DiscountAmount, f => f.Random.Decimal(0, 50))
            .RuleFor(i => i.TotalItemAmount, f => f.Random.Decimal(10, 500))
            .RuleFor(i => i.Status, SaleItemStatus.Active)
            .RuleFor(i => i.CreatedAt, f => f.Date.Recent(30))
            .RuleFor(i => i.UpdatedAt, f => f.Date.Recent(30))
            .RuleFor(i => i.CancelledAt, (f, i) => i.Status == SaleItemStatus.Cancelled ? f.Date.Recent(30) : null)
            .RuleFor(i => i.CancelledBy, (f, i) => i.Status == SaleItemStatus.Cancelled ? f.Random.Guid() : null);

        var item = itemFaker.Generate();
        item.CalculateTotalAmount();
        return item;
    }

    /// <summary>
    /// Creates a fake sale item with specific values
    /// </summary>
    public static SaleItem CreateFakeSaleItem(
        Guid saleId,
        Guid productId,
        int quantity,
        decimal unitPrice,
        SaleItemStatus status = SaleItemStatus.Active)
    {
        var item = CreateFakeSaleItem(saleId);
        item.ProductId = productId;
        item.Quantity = quantity;
        item.UnitPrice = unitPrice;
        item.Status = status;
        item.CalculateTotalAmount();
        return item;
    }

    /// <summary>
    /// Creates a list of fake sales
    /// </summary>
    public static List<Sale> CreateFakeSales(int count)
    {
        return new Faker<Sale>()
            .RuleFor(s => s.Id, f => Guid.NewGuid())
            .RuleFor(s => s.SaleNumber, f => $"SALE-{f.Random.Number(1000, 9999)}")
            .RuleFor(s => s.SaleDate, f => f.Date.Recent(30))
            .RuleFor(s => s.CustomerId, f => f.Random.Guid())
            .RuleFor(s => s.CustomerName, f => f.Person.FullName)
            .RuleFor(s => s.CustomerEmail, f => f.Person.Email)
            .RuleFor(s => s.CustomerPhone, f => f.Phone.PhoneNumber())
            .RuleFor(s => s.BranchId, f => f.Random.Guid())
            .RuleFor(s => s.BranchName, f => f.Company.CompanyName())
            .RuleFor(s => s.BranchCode, f => f.Random.AlphaNumeric(6).ToUpper())
            .RuleFor(s => s.TotalAmount, f => f.Random.Decimal(10, 1000))
            .RuleFor(s => s.Status, SaleStatus.Active)
            .RuleFor(s => s.CreatedAt, f => f.Date.Recent(30))
            .RuleFor(s => s.UpdatedAt, f => f.Date.Recent(30))
            .RuleFor(s => s.CancelledAt, (f, s) => s.Status == SaleStatus.Cancelled ? f.Date.Recent(30) : null)
            .RuleFor(s => s.CancelledBy, (f, s) => s.Status == SaleStatus.Cancelled ? f.Random.Guid() : null)
            .Generate(count);
    }

    /// <summary>
    /// Creates a list of fake sale items for a sale
    /// </summary>
    public static List<SaleItem> CreateFakeSaleItems(Guid saleId, int count)
    {
        var items = new List<SaleItem>();
        
        for (int i = 0; i < count; i++)
        {
            var item = CreateFakeSaleItem(saleId);
            items.Add(item);
        }
        
        return items;
    }

    /// <summary>
    /// Creates a fake sale with cancelled status for testing cancellation scenarios
    /// </summary>
    public static Sale CreateFakeCancelledSale()
    {
        var sale = CreateFakeSale();
        sale.Status = SaleStatus.Cancelled;
        sale.CancelledAt = DateTime.UtcNow;
        sale.CancelledBy = Guid.NewGuid();
        return sale;
    }

    /// <summary>
    /// Creates a fake sale item with cancelled status for testing cancellation scenarios
    /// </summary>
    public static SaleItem CreateFakeCancelledSaleItem(Guid saleId)
    {
        var item = CreateFakeSaleItem(saleId);
        item.Status = SaleItemStatus.Cancelled;
        item.CancelledAt = DateTime.UtcNow;
        item.CancelledBy = Guid.NewGuid();
        return item;
    }
}
