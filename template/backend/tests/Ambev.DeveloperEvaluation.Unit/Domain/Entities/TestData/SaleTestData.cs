using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Bogus;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class SaleTestData
{
    /// <summary>
    /// Configures the Faker to generate valid Sale entities.
    /// The generated sales will have valid:
    /// - SaleNumber (unique identifier)
    /// - SaleDate (current date)
    /// - CustomerId (valid GUID)
    /// - CustomerName (valid name)
    /// - CustomerEmail (valid email format)
    /// - CustomerPhone (Brazilian format)
    /// - BranchId (valid GUID)
    /// - BranchName (valid branch name)
    /// - BranchCode (valid branch code)
    /// - TotalAmount (positive decimal)
    /// - Status (Active or Cancelled)
    /// </summary>
    private static readonly Faker<Sale> SaleFaker = new Faker<Sale>()
        .RuleFor(s => s.Id, f => f.Random.Guid())
        .RuleFor(s => s.SaleNumber, f => $"SALE-{f.Random.Number(10000, 99999)}")
        .RuleFor(s => s.SaleDate, f => f.Date.Recent())
        .RuleFor(s => s.CustomerId, f => f.Random.Guid())
        .RuleFor(s => s.CustomerName, f => f.Person.FullName)
        .RuleFor(s => s.CustomerEmail, f => f.Person.Email)
        .RuleFor(s => s.CustomerPhone, f => $"+55{f.Random.Number(11, 99)}{f.Random.Number(100000000, 999999999)}")
        .RuleFor(s => s.BranchId, f => f.Random.Guid())
        .RuleFor(s => s.BranchName, f => f.Company.CompanyName())
        .RuleFor(s => s.BranchCode, f => $"BR-{f.Random.Number(100, 999)}")
        .RuleFor(s => s.TotalAmount, f => f.Random.Decimal(10.00m, 1000.00m))
        .RuleFor(s => s.Status, f => f.PickRandom(SaleStatus.Active, SaleStatus.Cancelled))
        .RuleFor(s => s.CreatedAt, f => f.Date.Recent())
        .RuleFor(s => s.UpdatedAt, f => f.Date.Recent().OrNull(f, 0.3f))
        .RuleFor(s => s.CancelledAt, f => f.Date.Recent().OrNull(f, 0.7f))
        .RuleFor(s => s.CancelledBy, f => f.Random.Guid().OrNull(f, 0.7f));

    /// <summary>
    /// Generates a valid Sale entity with randomized data.
    /// The generated sale will have all properties populated with valid values
    /// that meet the system's validation requirements.
    /// </summary>
    /// <returns>A valid Sale entity with randomly generated data.</returns>
    public static Sale GenerateValidSale()
    {
        var sale = SaleFaker.Generate();
        // Add at least one valid item to satisfy the validation requirement
        var item = SaleItemTestData.GenerateValidSaleItem();
        item.SaleId = sale.Id; // Ensure the item is properly associated with the sale
        sale.Items.Add(item);
        return sale;
    }

    /// <summary>
    /// Generates a valid sale number.
    /// The generated sale number will:
    /// - Follow the format SALE-XXXXX
    /// - Have 5 digits
    /// - Be unique for each generation
    /// </summary>
    /// <returns>A valid sale number.</returns>
    public static string GenerateValidSaleNumber()
    {
        return $"SALE-{new Faker().Random.Number(10000, 99999)}";
    }

    /// <summary>
    /// Generates a valid customer name.
    /// The generated name will:
    /// - Be a full name (first and last name)
    /// - Use realistic person names
    /// - Be between 5 and 50 characters
    /// </summary>
    /// <returns>A valid customer name.</returns>
    public static string GenerateValidCustomerName()
    {
        return new Faker().Person.FullName;
    }

    /// <summary>
    /// Generates a valid customer email.
    /// The generated email will:
    /// - Follow the standard email format (user@domain.com)
    /// - Have valid characters in both local and domain parts
    /// - Have a valid TLD
    /// </summary>
    /// <returns>A valid customer email.</returns>
    public static string GenerateValidCustomerEmail()
    {
        return new Faker().Internet.Email();
    }

    /// <summary>
    /// Generates a valid customer phone number.
    /// The generated phone number will:
    /// - Start with country code (+55)
    /// - Have a valid area code (11-99)
    /// - Have 9 digits for the phone number
    /// - Follow the format: +55XXXXXXXXXXXX
    /// </summary>
    /// <returns>A valid customer phone number.</returns>
    public static string GenerateValidCustomerPhone()
    {
        var faker = new Faker();
        return $"+55{faker.Random.Number(11, 99)}{faker.Random.Number(100000000, 999999999)}";
    }

    /// <summary>
    /// Generates a valid branch name.
    /// The generated branch name will:
    /// - Be a company name
    /// - Be between 5 and 100 characters
    /// - Use realistic company naming conventions
    /// </summary>
    /// <returns>A valid branch name.</returns>
    public static string GenerateValidBranchName()
    {
        return new Faker().Company.CompanyName();
    }

    /// <summary>
    /// Generates a valid branch code.
    /// The generated branch code will:
    /// - Follow the format BR-XXX
    /// - Have 3 digits
    /// - Be unique for each generation
    /// </summary>
    /// <returns>A valid branch code.</returns>
    public static string GenerateValidBranchCode()
    {
        return $"BR-{new Faker().Random.Number(100, 999)}";
    }

    /// <summary>
    /// Generates a valid total amount.
    /// The generated amount will:
    /// - Be a positive decimal value
    /// - Be between 10.00 and 1000.00
    /// - Have up to 2 decimal places
    /// </summary>
    /// <returns>A valid total amount.</returns>
    public static decimal GenerateValidTotalAmount()
    {
        return new Faker().Random.Decimal(10.00m, 1000.00m);
    }

    /// <summary>
    /// Generates an invalid sale number for testing negative scenarios.
    /// The generated sale number will:
    /// - Not follow the required format
    /// - Be empty or null
    /// - Be too short or too long
    /// This is useful for testing sale number validation error cases.
    /// </summary>
    /// <returns>An invalid sale number.</returns>
    public static string GenerateInvalidSaleNumber()
    {
        var faker = new Faker();
        return faker.PickRandom("", "INVALID", "SALE-", "SALE-123", faker.Random.String2(100));
    }

    /// <summary>
    /// Generates an invalid customer name for testing negative scenarios.
    /// The generated name will:
    /// - Be empty or null
    /// - Be too short (less than 2 characters)
    /// - Be too long (more than 100 characters)
    /// This is useful for testing customer name validation error cases.
    /// </summary>
    /// <returns>An invalid customer name.</returns>
    public static string GenerateInvalidCustomerName()
    {
        var faker = new Faker();
        return faker.PickRandom("", "A", faker.Random.String2(101));
    }

    /// <summary>
    /// Generates an invalid customer email for testing negative scenarios.
    /// The generated email will:
    /// - Not follow the standard email format
    /// - Not contain the @ symbol
    /// - Be a simple word or string
    /// This is useful for testing email validation error cases.
    /// </summary>
    /// <returns>An invalid customer email.</returns>
    public static string GenerateInvalidCustomerEmail()
    {
        var faker = new Faker();
        return faker.PickRandom("", "invalid-email", "test@", "@test.com", faker.Lorem.Word());
    }

    /// <summary>
    /// Generates an invalid customer phone for testing negative scenarios.
    /// The generated phone number will:
    /// - Not follow the Brazilian phone number format
    /// - Not have the correct length
    /// - Not start with the country code
    /// This is useful for testing phone validation error cases.
    /// </summary>
    /// <returns>An invalid customer phone number.</returns>
    public static string GenerateInvalidCustomerPhone()
    {
        var faker = new Faker();
        return faker.PickRandom("", "123", "invalid-phone", faker.Random.AlphaNumeric(5));
    }

    /// <summary>
    /// Generates an invalid total amount for testing negative scenarios.
    /// The generated amount will:
    /// - Be negative
    /// - Be zero
    /// - Be extremely large
    /// This is useful for testing amount validation error cases.
    /// </summary>
    /// <returns>An invalid total amount.</returns>
    public static decimal GenerateInvalidTotalAmount()
    {
        var faker = new Faker();
        return faker.PickRandom(-100.00m, 0.00m, 999999999.99m);
    }
}
