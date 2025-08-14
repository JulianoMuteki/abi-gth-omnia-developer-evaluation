using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class SaleItemTestData
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
    /// - Status (Active or Cancelled)
    /// </summary>
    private static readonly Faker<SaleItem> SaleItemFaker = new Faker<SaleItem>()
        .RuleFor(si => si.Id, f => f.Random.Guid())
        .RuleFor(si => si.SaleId, f => f.Random.Guid())
        .RuleFor(si => si.ProductId, f => f.Random.Guid())
        .RuleFor(si => si.ProductName, f => f.Commerce.ProductName())
        .RuleFor(si => si.ProductCode, f => $"PROD-{f.Random.Number(1000, 9999)}")
        .RuleFor(si => si.ProductDescription, f => f.Commerce.ProductDescription())
        .RuleFor(si => si.Quantity, f => f.Random.Number(1, 20))
        .RuleFor(si => si.UnitPrice, f => f.Random.Decimal(1.00m, 100.00m))
        .RuleFor(si => si.Status, f => f.PickRandom(SaleItemStatus.Active, SaleItemStatus.Cancelled))
        .RuleFor(si => si.CreatedAt, f => f.Date.Recent())
        .RuleFor(si => si.UpdatedAt, f => f.Date.Recent().OrNull(f, 0.3f))
        .RuleFor(si => si.CancelledAt, f => f.Date.Recent().OrNull(f, 0.7f))
        .RuleFor(si => si.CancelledBy, f => f.Random.Guid().OrNull(f, 0.7f));

    /// <summary>
    /// Generates a valid SaleItem entity with randomized data.
    /// The generated sale item will have all properties populated with valid values
    /// that meet the system's validation requirements.
    /// </summary>
    /// <returns>A valid SaleItem entity with randomly generated data.</returns>
    public static SaleItem GenerateValidSaleItem()
    {
        var saleItem = SaleItemFaker.Generate();
        saleItem.CalculateTotalAmount(); // Calculate totals after generation
        return saleItem;
    }

    /// <summary>
    /// Generates a valid product name.
    /// The generated product name will:
    /// - Be a realistic product name
    /// - Be between 5 and 100 characters
    /// - Use commerce product naming conventions
    /// </summary>
    /// <returns>A valid product name.</returns>
    public static string GenerateValidProductName()
    {
        return new Faker().Commerce.ProductName();
    }

    /// <summary>
    /// Generates a valid product code.
    /// The generated product code will:
    /// - Follow the format PROD-XXXX
    /// - Have 4 digits
    /// - Be unique for each generation
    /// </summary>
    /// <returns>A valid product code.</returns>
    public static string GenerateValidProductCode()
    {
        return $"PROD-{new Faker().Random.Number(1000, 9999)}";
    }

    /// <summary>
    /// Generates a valid product description.
    /// The generated description will:
    /// - Be a realistic product description
    /// - Be between 10 and 500 characters
    /// - Use commerce product description conventions
    /// </summary>
    /// <returns>A valid product description.</returns>
    public static string GenerateValidProductDescription()
    {
        return new Faker().Commerce.ProductDescription();
    }

    /// <summary>
    /// Generates a valid quantity.
    /// The generated quantity will:
    /// - Be between 1 and 20 (business rule)
    /// - Be a positive integer
    /// - Be within acceptable range
    /// </summary>
    /// <returns>A valid quantity.</returns>
    public static int GenerateValidQuantity()
    {
        return new Faker().Random.Number(1, 20);
    }

    /// <summary>
    /// Generates a valid unit price.
    /// The generated unit price will:
    /// - Be a positive decimal value
    /// - Be between 1.00 and 100.00
    /// - Have up to 2 decimal places
    /// </summary>
    /// <returns>A valid unit price.</returns>
    public static decimal GenerateValidUnitPrice()
    {
        return new Faker().Random.Decimal(1.00m, 100.00m);
    }

    /// <summary>
    /// Generates a valid quantity that qualifies for 10% discount.
    /// The generated quantity will:
    /// - Be between 4 and 9
    /// - Qualify for 10% discount according to business rules
    /// </summary>
    /// <returns>A valid quantity for 10% discount.</returns>
    public static int GenerateQuantityFor10PercentDiscount()
    {
        return new Faker().Random.Number(4, 9);
    }

    /// <summary>
    /// Generates a valid quantity that qualifies for 20% discount.
    /// The generated quantity will:
    /// - Be between 10 and 20
    /// - Qualify for 20% discount according to business rules
    /// </summary>
    /// <returns>A valid quantity for 20% discount.</returns>
    public static int GenerateQuantityFor20PercentDiscount()
    {
        return new Faker().Random.Number(10, 20);
    }

    /// <summary>
    /// Generates a valid quantity that has no discount.
    /// The generated quantity will:
    /// - Be between 1 and 3
    /// - Have no discount according to business rules
    /// </summary>
    /// <returns>A valid quantity with no discount.</returns>
    public static int GenerateQuantityWithNoDiscount()
    {
        return new Faker().Random.Number(1, 3);
    }

    /// <summary>
    /// Generates an invalid product name for testing negative scenarios.
    /// The generated product name will:
    /// - Be empty or null
    /// - Be too short (less than 2 characters)
    /// - Be too long (more than 200 characters)
    /// This is useful for testing product name validation error cases.
    /// </summary>
    /// <returns>An invalid product name.</returns>
    public static string GenerateInvalidProductName()
    {
        var faker = new Faker();
        return faker.PickRandom("", "A", faker.Random.String2(201));
    }

    /// <summary>
    /// Generates an invalid product code for testing negative scenarios.
    /// The generated product code will:
    /// - Not follow the required format
    /// - Be empty or null
    /// - Be too short or too long
    /// This is useful for testing product code validation error cases.
    /// </summary>
    /// <returns>An invalid product code.</returns>
    public static string GenerateInvalidProductCode()
    {
        var faker = new Faker();
        return faker.PickRandom("", "INVALID", "PROD-", "PROD-123", faker.Random.String2(100));
    }

    /// <summary>
    /// Generates an invalid quantity for testing negative scenarios.
    /// The generated quantity will:
    /// - Be zero or negative
    /// - Be greater than 20 (business rule violation)
    /// - Be outside the acceptable range
    /// This is useful for testing quantity validation error cases.
    /// </summary>
    /// <returns>An invalid quantity.</returns>
    public static int GenerateInvalidQuantity()
    {
        var faker = new Faker();
        return faker.PickRandom(0, -1, -5, 21, 25, 100);
    }

    /// <summary>
    /// Generates an invalid unit price for testing negative scenarios.
    /// The generated unit price will:
    /// - Be negative
    /// - Be zero
    /// - Be extremely large
    /// This is useful for testing unit price validation error cases.
    /// </summary>
    /// <returns>An invalid unit price.</returns>
    public static decimal GenerateInvalidUnitPrice()
    {
        var faker = new Faker();
        return faker.PickRandom(-50.00m, 0.00m, 999999999.99m);
    }
}
