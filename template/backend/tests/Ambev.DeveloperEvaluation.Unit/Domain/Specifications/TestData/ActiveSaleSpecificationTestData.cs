using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Specifications.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation for ActiveSaleSpecification tests
/// to ensure consistency across test cases.
/// </summary>
public static class ActiveSaleSpecificationTestData
{
    /// <summary>
    /// Configures the Faker to generate valid Sale entities.
    /// The generated sales will have valid:
    /// - SaleNumber (SALE-XXXXX format)
    /// - SaleDate (current date)
    /// - CustomerId (valid GUID)
    /// - CustomerName (valid name)
    /// - CustomerEmail (valid email format)
    /// - CustomerPhone (Brazilian format)
    /// - BranchId (valid GUID)
    /// - BranchName (valid branch name)
    /// - BranchCode (valid branch code)
    /// - TotalAmount (positive decimal)
    /// Status is not set here as it's the main test parameter
    /// </summary>
    private static readonly Faker<Sale> SaleFaker = new Faker<Sale>()
        .CustomInstantiator(f => new Sale
        {
            SaleNumber = $"SALE-{f.Random.Number(10000, 99999)}",
            SaleDate = f.Date.Recent(),
            CustomerId = f.Random.Guid(),
            CustomerName = f.Person.FullName,
            CustomerEmail = f.Person.Email,
            CustomerPhone = $"+55{f.Random.Number(11, 99)}{f.Random.Number(100000000, 999999999)}",
            BranchId = f.Random.Guid(),
            BranchName = f.Company.CompanyName(),
            BranchCode = $"BR-{f.Random.Number(100, 999)}",
            TotalAmount = f.Random.Decimal(10.00m, 1000.00m),
            Status = f.PickRandom<SaleStatus>(),
            CreatedAt = f.Date.Recent(),
            UpdatedAt = f.Date.Recent().OrNull(f, 0.3f),
            CancelledAt = f.Date.Recent().OrNull(f, 0.7f),
            CancelledBy = f.Random.Guid().OrNull(f, 0.7f)
        });

    /// <summary>
    /// Generates a valid Sale entity with the specified status.
    /// </summary>
    /// <param name="status">The SaleStatus to set for the generated sale.</param>
    /// <returns>A valid Sale entity with randomly generated data and specified status.</returns>
    public static Sale GenerateSale(SaleStatus status)
    {
        var sale = SaleFaker.Generate();
        sale.Status = status;
        return sale;
    }
}
