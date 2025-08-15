using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

/// <summary>
/// Test data generator for CreateSaleHandler tests.
/// </summary>
public static class CreateSaleHandlerTestData
{
    private static readonly Faker _faker = new Faker();

    /// <summary>
    /// Generates a valid CreateSaleCommand for testing.
    /// </summary>
    /// <returns>A valid CreateSaleCommand instance.</returns>
    public static CreateSaleCommand GenerateValidCommand()
    {
        return new Faker<CreateSaleCommand>()
            .RuleFor(c => c.SaleNumber, f => $"SALE-{f.Random.Number(1000, 9999)}")
            .RuleFor(c => c.SaleDate, f => f.Date.Recent(30))
            .RuleFor(c => c.CustomerId, f => f.Random.Guid())
            .RuleFor(c => c.CustomerName, f => f.Person.FullName)
            .RuleFor(c => c.CustomerEmail, f => f.Person.Email)
            .RuleFor(c => c.CustomerPhone, f => LimitPhoneLength(f.Phone.PhoneNumber()))
            .RuleFor(c => c.BranchId, f => f.Random.Guid())
            .RuleFor(c => c.BranchName, f => f.Company.CompanyName())
            .RuleFor(c => c.BranchCode, f => f.Random.AlphaNumeric(5).ToUpper())
            .RuleFor(c => c.Status, SaleStatus.Active)
            .RuleFor(c => c.Items, f => GenerateValidItems(f.Random.Number(1, 3)))
            .Generate();
    }

    /// <summary>
    /// Generates a valid Sale entity for testing.
    /// </summary>
    /// <returns>A valid Sale instance.</returns>
    public static Sale GenerateSale()
    {
        var sale = new Faker<Sale>()
            .RuleFor(s => s.Id, f => f.Random.Guid())
            .RuleFor(s => s.SaleNumber, f => $"SALE-{f.Random.Number(1000, 9999)}")
            .RuleFor(s => s.SaleDate, f => f.Date.Recent(30))
            .RuleFor(s => s.CustomerId, f => f.Random.Guid())
            .RuleFor(s => s.CustomerName, f => f.Person.FullName)
            .RuleFor(s => s.CustomerEmail, f => f.Person.Email)
            .RuleFor(s => s.CustomerPhone, f => LimitPhoneLength(f.Phone.PhoneNumber()))
            .RuleFor(s => s.BranchId, f => f.Random.Guid())
            .RuleFor(s => s.BranchName, f => f.Company.CompanyName())
            .RuleFor(s => s.BranchCode, f => f.Random.AlphaNumeric(5).ToUpper())
            .RuleFor(s => s.Status, SaleStatus.Active)
            .RuleFor(s => s.TotalAmount, f => f.Random.Decimal(100, 1000))
            .RuleFor(s => s.CreatedAt, f => f.Date.Recent(30))
            .RuleFor(s => s.UpdatedAt, f => f.Date.Recent(30))
            .Generate();

        sale.Items = GenerateValidSaleItems(_faker.Random.Number(1, 3));
        return sale;
    }

    /// <summary>
    /// Generates a valid CreateSaleResult for testing.
    /// </summary>
    /// <returns>A valid CreateSaleResult instance.</returns>
    public static CreateSaleResult GenerateResult()
    {
        return new Faker<CreateSaleResult>()
            .RuleFor(r => r.Id, f => f.Random.Guid())
            .RuleFor(r => r.SaleNumber, f => $"SALE-{f.Random.Number(1000, 9999)}")
            .RuleFor(r => r.SaleDate, f => f.Date.Recent(30))
            .RuleFor(r => r.CustomerId, f => f.Random.Guid())
            .RuleFor(r => r.CustomerName, f => f.Person.FullName)
            .RuleFor(r => r.CustomerEmail, f => f.Person.Email)
            .RuleFor(r => r.CustomerPhone, f => LimitPhoneLength(f.Phone.PhoneNumber()))
            .RuleFor(r => r.BranchId, f => f.Random.Guid())
            .RuleFor(r => r.BranchName, f => f.Company.CompanyName())
            .RuleFor(r => r.BranchCode, f => f.Random.AlphaNumeric(5).ToUpper())
            .RuleFor(r => r.Status, SaleStatus.Active)
            .RuleFor(r => r.TotalAmount, f => f.Random.Decimal(100, 1000))
            .RuleFor(r => r.Items, f => GenerateValidResultItems(f.Random.Number(1, 3)))
            .RuleFor(r => r.CreatedAt, f => f.Date.Recent(30))
            .Generate();
    }

    private static List<CreateSaleItemCommand> GenerateValidItems(int count)
    {
        return new Faker<CreateSaleItemCommand>()
            .RuleFor(i => i.ProductId, f => f.Random.Guid())
            .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
            .RuleFor(i => i.ProductCode, f => f.Random.AlphaNumeric(8).ToUpper())
            .RuleFor(i => i.ProductDescription, f => f.Lorem.Sentence())
            .RuleFor(i => i.Quantity, f => f.Random.Number(1, 10))
            .RuleFor(i => i.UnitPrice, f => f.Random.Decimal(10, 100))
            .RuleFor(i => i.DiscountPercentage, f => f.Random.Decimal(0, 20))
            .RuleFor(i => i.Status, SaleItemStatus.Active)
            .Generate(count);
    }

    private static List<SaleItem> GenerateValidSaleItems(int count)
    {
        return new Faker<SaleItem>()
            .RuleFor(i => i.Id, f => f.Random.Guid())
            .RuleFor(i => i.ProductId, f => f.Random.Guid())
            .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
            .RuleFor(i => i.ProductCode, f => f.Random.AlphaNumeric(8).ToUpper())
            .RuleFor(i => i.ProductDescription, f => f.Lorem.Sentence())
            .RuleFor(i => i.Quantity, f => f.Random.Number(1, 10))
            .RuleFor(i => i.UnitPrice, f => f.Random.Decimal(10, 100))
            .RuleFor(i => i.DiscountPercentage, f => f.Random.Decimal(0, 20))
            .RuleFor(i => i.TotalItemAmount, f => f.Random.Decimal(50, 500))
            .RuleFor(i => i.Status, SaleItemStatus.Active)
            .RuleFor(i => i.CreatedAt, f => f.Date.Recent(30))
            .RuleFor(i => i.UpdatedAt, f => f.Date.Recent(30))
            .Generate(count);
    }

    private static List<CreateSaleItemResult> GenerateValidResultItems(int count)
    {
        return new Faker<CreateSaleItemResult>()
            .RuleFor(i => i.Id, f => f.Random.Guid())
            .RuleFor(i => i.ProductId, f => f.Random.Guid())
            .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
            .RuleFor(i => i.ProductCode, f => f.Random.AlphaNumeric(8).ToUpper())
            .RuleFor(i => i.ProductDescription, f => f.Lorem.Sentence())
            .RuleFor(i => i.Quantity, f => f.Random.Number(1, 10))
            .RuleFor(i => i.UnitPrice, f => f.Random.Decimal(10, 100))
            .RuleFor(i => i.DiscountPercentage, f => f.Random.Decimal(0, 20))
            .RuleFor(i => i.TotalItemAmount, f => f.Random.Decimal(50, 500))
            .RuleFor(i => i.Status, SaleItemStatus.Active)
            .RuleFor(i => i.CreatedAt, f => f.Date.Recent(30))
            .Generate(count);
    }

    private static string LimitPhoneLength(string phone)
    {
        return phone.Length > 20 ? phone.Substring(0, 20) : phone;
    }
}
