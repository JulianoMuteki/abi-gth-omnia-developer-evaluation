using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

/// <summary>
/// Test data generator for DeleteSaleHandler tests.
/// </summary>
public static class DeleteSaleHandlerTestData
{
    /// <summary>
    /// Generates a valid DeleteSaleCommand for testing.
    /// </summary>
    /// <returns>A valid DeleteSaleCommand instance.</returns>
    public static DeleteSaleCommand GenerateValidCommand()
    {
        return new Faker<DeleteSaleCommand>()
            .RuleFor(c => c.Id, f => f.Random.Guid())
            .Generate();
    }

    /// <summary>
    /// Generates a valid Sale entity for testing.
    /// </summary>
    /// <returns>A valid Sale instance.</returns>
    public static Sale GenerateSale()
    {
        return new Faker<Sale>()
            .RuleFor(s => s.Id, f => f.Random.Guid())
            .RuleFor(s => s.SaleNumber, f => $"SALE-{f.Random.Number(1000, 9999)}")
            .RuleFor(s => s.SaleDate, f => f.Date.Recent(30))
            .RuleFor(s => s.CustomerId, f => f.Random.Guid())
            .RuleFor(s => s.CustomerName, f => f.Person.FullName)
            .RuleFor(s => s.CustomerEmail, f => f.Person.Email)
            .RuleFor(s => s.CustomerPhone, f => f.Phone.PhoneNumber())
            .RuleFor(s => s.BranchId, f => f.Random.Guid())
            .RuleFor(s => s.BranchName, f => f.Company.CompanyName())
            .RuleFor(s => s.BranchCode, f => f.Random.AlphaNumeric(5).ToUpper())
            .RuleFor(s => s.Status, f => f.PickRandom<SaleStatus>())
            .RuleFor(s => s.TotalAmount, f => f.Random.Decimal(100, 1000))
            .RuleFor(s => s.CreatedAt, f => f.Date.Recent(30))
            .RuleFor(s => s.UpdatedAt, f => f.Date.Recent(30))
            .Generate();
    }
}
