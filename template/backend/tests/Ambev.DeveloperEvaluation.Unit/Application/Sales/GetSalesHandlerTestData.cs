using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

/// <summary>
/// Test data generator for GetSalesHandler tests.
/// </summary>
public static class GetSalesHandlerTestData
{
    private static readonly Faker _faker = new Faker();

    /// <summary>
    /// Generates a valid GetSalesCommand for testing.
    /// </summary>
    /// <returns>A valid GetSalesCommand instance.</returns>
    public static GetSalesCommand GenerateValidCommand()
    {
        return new Faker<GetSalesCommand>()
            .RuleFor(c => c.PageNumber, f => f.Random.Number(1, 10))
            .RuleFor(c => c.PageSize, f => f.Random.Number(5, 20))
            .RuleFor(c => c.CustomerId, f => f.Random.Bool() ? f.Random.Guid() : null)
            .RuleFor(c => c.BranchId, f => f.Random.Bool() ? f.Random.Guid() : null)
            .RuleFor(c => c.StartDate, f => f.Random.Bool() ? f.Date.Past(30) : null)
            .RuleFor(c => c.EndDate, f => f.Random.Bool() ? f.Date.Recent(30) : null)
            .RuleFor(c => c.Status, f => f.Random.Bool() ? f.PickRandom<SaleStatus>().ToString() : null)
            .Generate();
    }

    /// <summary>
    /// Generates a list of Sale entities for testing.
    /// </summary>
    /// <param name="count">The number of sales to generate.</param>
    /// <returns>A list of Sale instances.</returns>
    public static List<Sale> GenerateSales(int count)
    {
        return new Faker<Sale>()
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
            .RuleFor(s => s.Status, f => f.PickRandom<SaleStatus>())
            .RuleFor(s => s.TotalAmount, f => f.Random.Decimal(100, 1000))
            .RuleFor(s => s.CreatedAt, f => f.Date.Recent(30))
            .RuleFor(s => s.UpdatedAt, f => f.Date.Recent(30))
            .RuleFor(s => s.Items, f => GenerateValidSaleItems(_faker.Random.Number(1, 3)))
            .Generate(count);
    }

    /// <summary>
    /// Generates a list of GetSalesItemResult for testing.
    /// </summary>
    /// <param name="count">The number of items to generate.</param>
    /// <returns>A list of GetSalesItemResult instances.</returns>
    public static List<GetSalesItemResult> GenerateSalesItems(int count)
    {
        return new Faker<GetSalesItemResult>()
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
            .RuleFor(s => s.Status, f => f.PickRandom<SaleStatus>())
            .RuleFor(s => s.TotalAmount, f => f.Random.Decimal(100, 1000))
            .RuleFor(s => s.ItemCount, f => f.Random.Number(1, 5))
            .RuleFor(s => s.CreatedAt, f => f.Date.Recent(30))
            .RuleFor(s => s.UpdatedAt, f => f.Date.Recent(30))
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
            .RuleFor(i => i.Status, f => f.PickRandom<SaleItemStatus>())
            .RuleFor(i => i.CreatedAt, f => f.Date.Recent(30))
            .RuleFor(i => i.UpdatedAt, f => f.Date.Recent(30))
            .Generate(count);
    }

    private static string LimitPhoneLength(string phone)
    {
        return phone.Length > 20 ? phone.Substring(0, 20) : phone;
    }
}
