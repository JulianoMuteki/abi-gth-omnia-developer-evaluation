using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

/// <summary>
/// Test data generator for CreateSaleValidator tests.
/// </summary>
public static class CreateSaleValidatorTestData
{
    /// <summary>
    /// Generates a valid CreateSaleCommand for testing.
    /// </summary>
    /// <returns>A valid CreateSaleCommand instance.</returns>
    public static CreateSaleCommand GenerateValidCommand()
    {
        return new Faker<CreateSaleCommand>()
            .RuleFor(c => c.SaleNumber, f => $"SALE-{f.Random.Number(1000, 9999)}")
            .RuleFor(c => c.SaleDate, f => f.Date.Past(30))
            .RuleFor(c => c.CustomerId, f => f.Random.Guid())
            .RuleFor(c => c.CustomerName, f => f.Person.FullName)
            .RuleFor(c => c.CustomerEmail, f => f.Person.Email)
            .RuleFor(c => c.CustomerPhone, f => f.Phone.PhoneNumber())
            .RuleFor(c => c.BranchId, f => f.Random.Guid())
            .RuleFor(c => c.BranchName, f => f.Company.CompanyName())
            .RuleFor(c => c.BranchCode, f => f.Random.AlphaNumeric(5).ToUpper())
            .RuleFor(c => c.Status, SaleStatus.Active)
            .RuleFor(c => c.Items, f => GenerateValidItems(f.Random.Number(1, 3)))
            .Generate();
    }

    /// <summary>
    /// Generates a valid CreateSaleItemCommand for testing.
    /// </summary>
    /// <returns>A valid CreateSaleItemCommand instance.</returns>
    public static CreateSaleItemCommand GenerateValidItemCommand()
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
}
