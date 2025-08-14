using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Validation;

/// <summary>
/// Contains unit tests for the SaleValidator class.
/// Tests cover validation of all sale properties including sale number, customer information,
/// branch information, and business rules.
/// </summary>
public class SaleValidatorTests
{
    private readonly SaleValidator _validator;

    public SaleValidatorTests()
    {
        _validator = new SaleValidator();
    }

    /// <summary>
    /// Tests that validation passes when all sale properties are valid.
    /// This test verifies that a sale with valid:
    /// - SaleNumber (SALE-XXXXX format)
    /// - SaleDate (not in future)
    /// - CustomerId (valid GUID)
    /// - CustomerName (not empty, max 100 chars)
    /// - CustomerEmail (valid email format)
    /// - CustomerPhone (valid Brazilian format)
    /// - BranchId (valid GUID)
    /// - BranchName (not empty, max 100 chars)
    /// - BranchCode (BR-XXX format)
    /// - TotalAmount (non-negative)
    /// - Status (Active/Cancelled)
    /// - Items (at least one item)
    /// passes all validation rules without any errors.
    /// </summary>
    [Fact(DisplayName = "Valid sale should pass all validation rules")]
    public void Given_ValidSale_When_Validated_Then_ShouldNotHaveErrors()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        var result = _validator.TestValidate(sale);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    /// <summary>
    /// Tests that validation fails for invalid sale number formats.
    /// This test verifies that sale numbers that:
    /// - Are empty strings
    /// - Don't follow the SALE-XXXXX format
    /// - Are too long
    /// fail validation with appropriate error messages.
    /// </summary>
    /// <param name="saleNumber">The invalid sale number to test.</param>
    [Theory(DisplayName = "Invalid sale number formats should fail validation")]
    [InlineData("")] // Empty
    [InlineData("INVALID")] // Wrong format
    [InlineData("SALE-")] // Incomplete format
    [InlineData("SALE-123")] // Too short
    [InlineData("SALE-123456")] // Too long
    [InlineData("SALE-ABC12")] // Contains letters
    public void Given_InvalidSaleNumber_When_Validated_Then_ShouldHaveError(string saleNumber)
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.SaleNumber = saleNumber;

        // Act
        var result = _validator.TestValidate(sale);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SaleNumber);
    }

    /// <summary>
    /// Tests that validation fails when sale date is in the future.
    /// </summary>
    [Fact(DisplayName = "Sale date in the future should fail validation")]
    public void Given_SaleDateInFuture_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.SaleDate = DateTime.UtcNow.AddDays(1);

        // Act
        var result = _validator.TestValidate(sale);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SaleDate);
    }

    /// <summary>
    /// Tests that validation fails when customer name exceeds maximum length.
    /// </summary>
    [Fact(DisplayName = "Customer name longer than maximum length should fail validation")]
    public void Given_CustomerNameLongerThanMaximum_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.CustomerName = new string('A', 101); // 101 characters

        // Act
        var result = _validator.TestValidate(sale);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CustomerName);
    }

    /// <summary>
    /// Tests that validation fails for invalid customer email formats.
    /// </summary>
    /// <param name="customerEmail">The invalid customer email to test.</param>
    [Theory(DisplayName = "Invalid customer email formats should fail validation")]
    [InlineData("")] // Empty
    [InlineData("invalid-email")] // No @ symbol
    [InlineData("test@")] // No domain
    [InlineData("@test.com")] // No local part
    public void Given_InvalidCustomerEmail_When_Validated_Then_ShouldHaveError(string customerEmail)
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.CustomerEmail = customerEmail;

        // Act
        var result = _validator.TestValidate(sale);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CustomerEmail);
    }

    /// <summary>
    /// Tests that validation fails when customer email exceeds maximum length.
    /// </summary>
    [Fact(DisplayName = "Customer email longer than maximum length should fail validation")]
    public void Given_CustomerEmailLongerThanMaximum_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.CustomerEmail = new string('a', 100) + "@test.com"; // 101 characters total

        // Act
        var result = _validator.TestValidate(sale);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CustomerEmail);
    }

    /// <summary>
    /// Tests that validation fails for invalid customer phone formats.
    /// </summary>
    /// <param name="customerPhone">The invalid customer phone to test.</param>
    [Theory(DisplayName = "Invalid customer phone formats should fail validation")]
    [InlineData("")] // Empty
    [InlineData("123")] // Too short
    [InlineData("invalid-phone")] // Wrong format
    [InlineData("+55123456789")] // Too short
    [InlineData("+551234567890123")] // Too long
    [InlineData("551234567890")] // Missing +
    public void Given_InvalidCustomerPhone_When_Validated_Then_ShouldHaveError(string customerPhone)
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.CustomerPhone = customerPhone;

        // Act
        var result = _validator.TestValidate(sale);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CustomerPhone);
    }

    /// <summary>
    /// Tests that validation fails for invalid branch name formats.
    /// </summary>
    /// <param name="branchName">The invalid branch name to test.</param>
    [Theory(DisplayName = "Invalid branch name formats should fail validation")]
    [InlineData("")] // Empty
    public void Given_InvalidBranchName_When_Validated_Then_ShouldHaveError(string branchName)
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.BranchName = branchName;

        // Act
        var result = _validator.TestValidate(sale);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BranchName);
    }

    /// <summary>
    /// Tests that validation fails when branch name exceeds maximum length.
    /// </summary>
    [Fact(DisplayName = "Branch name longer than maximum length should fail validation")]
    public void Given_BranchNameLongerThanMaximum_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.BranchName = new string('A', 101); // 101 characters

        // Act
        var result = _validator.TestValidate(sale);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BranchName);
    }

    /// <summary>
    /// Tests that validation fails for invalid branch code formats.
    /// </summary>
    /// <param name="branchCode">The invalid branch code to test.</param>
    [Theory(DisplayName = "Invalid branch code formats should fail validation")]
    [InlineData("")] // Empty
    [InlineData("INVALID")] // Wrong format
    [InlineData("BR-")] // Incomplete format
    [InlineData("BR-12")] // Too short
    [InlineData("BR-1234")] // Too long
    [InlineData("BR-ABC")] // Contains letters
    public void Given_InvalidBranchCode_When_Validated_Then_ShouldHaveError(string branchCode)
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.BranchCode = branchCode;

        // Act
        var result = _validator.TestValidate(sale);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BranchCode);
    }

    /// <summary>
    /// Tests that validation fails when total amount is negative.
    /// </summary>
    [Fact(DisplayName = "Negative total amount should fail validation")]
    public void Given_NegativeTotalAmount_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.TotalAmount = -100.00m;

        // Act
        var result = _validator.TestValidate(sale);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TotalAmount);
    }

    /// <summary>
    /// Tests that validation fails when sale status is Unknown.
    /// </summary>
    [Fact(DisplayName = "Unknown sale status should fail validation")]
    public void Given_UnknownSaleStatus_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Status = SaleStatus.Unknown;

        // Act
        var result = _validator.TestValidate(sale);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    /// <summary>
    /// Tests that validation fails when sale has no items.
    /// </summary>
    [Fact(DisplayName = "Sale with no items should fail validation")]
    public void Given_SaleWithNoItems_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Items.Clear();

        // Act
        var result = _validator.TestValidate(sale);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Items);
    }

    /// <summary>
    /// Tests that validation passes for valid sale statuses.
    /// </summary>
    /// <param name="status">The valid sale status to test.</param>
    [Theory(DisplayName = "Valid sale statuses should pass validation")]
    [InlineData(SaleStatus.Active)]
    [InlineData(SaleStatus.Cancelled)]
    public void Given_ValidSaleStatus_When_Validated_Then_ShouldNotHaveError(SaleStatus status)
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Status = status;

        // Act
        var result = _validator.TestValidate(sale);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }

    /// <summary>
    /// Tests that validation passes for zero total amount.
    /// </summary>
    [Fact(DisplayName = "Zero total amount should pass validation")]
    public void Given_ZeroTotalAmount_When_Validated_Then_ShouldNotHaveError()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.TotalAmount = 0.00m;

        // Act
        var result = _validator.TestValidate(sale);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.TotalAmount);
    }
}
