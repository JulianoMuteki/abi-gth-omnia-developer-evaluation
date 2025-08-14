using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Specifications.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation for ActiveSaleItemSpecification tests
/// to ensure consistency across test cases.
/// </summary>
public static class ActiveSaleItemSpecificationTestData
{
    /// <summary>
    /// Configures the Faker to generate valid SaleItem entities.
    /// The generated sale items will have valid:
    /// - SaleId (valid GUID)
    /// - ProductId (valid GUID)
    /// - ProductName (valid product name)
    /// - ProductCode (valid product code)
    /// - ProductDescription (valid description)
    /// - Quantity (between 1 and 20)
    /// - UnitPrice (positive decimal)
    /// - DiscountPercentage (calculated based on quantity)
    /// - DiscountAmount (calculated)
    /// - TotalItemAmount (calculated)
    /// Status is not set here as it's the main test parameter
    /// </summary>
    private static readonly Faker<SaleItem> SaleItemFaker = new Faker<SaleItem>()
        .CustomInstantiator(f => new SaleItem
        {
            SaleId = f.Random.Guid(),
            ProductId = f.Random.Guid(),
            ProductName = f.Commerce.ProductName(),
            ProductCode = $"PROD-{f.Random.Number(1000, 9999)}",
            ProductDescription = f.Commerce.ProductDescription(),
            Quantity = f.Random.Number(1, 20),
            UnitPrice = f.Random.Decimal(1.00m, 100.00m),
            Status = f.PickRandom<SaleItemStatus>(),
            CreatedAt = f.Date.Recent(),
            UpdatedAt = f.Date.Recent().OrNull(f, 0.3f),
            CancelledAt = f.Date.Recent().OrNull(f, 0.7f),
            CancelledBy = f.Random.Guid().OrNull(f, 0.7f)
        });

    /// <summary>
    /// Generates a valid SaleItem entity with the specified status.
    /// </summary>
    /// <param name="status">The SaleItemStatus to set for the generated sale item.</param>
    /// <returns>A valid SaleItem entity with randomly generated data and specified status.</returns>
    public static SaleItem GenerateSaleItem(SaleItemStatus status)
    {
        var saleItem = SaleItemFaker.Generate();
        saleItem.Status = status;
        saleItem.CalculateTotalAmount(); // Calculate totals after generation
        return saleItem;
    }
}
