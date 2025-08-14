using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Validation;

/// <summary>
/// Contains unit tests for the SaleItemValidator class.
/// Tests cover validation of all sale item properties including product information,
/// quantity, pricing, and business rules.
/// </summary>
public class SaleItemValidatorTests
{
    private readonly SaleItemValidator _validator;

    public SaleItemValidatorTests()
    {
        _validator = new SaleItemValidator();
    }

    /// <summary>
    /// Tests that validation passes when all sale item properties are valid.
    /// This test verifies that a sale item with valid:
    /// - SaleId (valid GUID)
    /// - ProductId (valid GUID)
    /// - ProductName (not empty, max 100 chars)
    /// - ProductCode (PROD-XXXX format)
    /// - ProductDescription (max 500 chars)
    /// - Quantity (1-20)
    /// - UnitPrice (non-negative)
    /// - DiscountPercentage (0-100)
    /// - DiscountAmount (non-negative)
    /// - TotalItemAmount (non-negative)
    /// - Status (Active/Cancelled)
    /// passes all validation rules without any errors.
    /// </summary>
    [Fact(DisplayName = "Valid sale item should pass all validation rules")]
    public void Given_ValidSaleItem_When_Validated_Then_ShouldNotHaveErrors()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();

        // Act
        var result = _validator.TestValidate(saleItem);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    /// <summary>
    /// Tests that validation fails for invalid product name formats.
    /// </summary>
    /// <param name="productName">The invalid product name to test.</param>
    [Theory(DisplayName = "Invalid product name formats should fail validation")]
    [InlineData("")] // Empty
    public void Given_InvalidProductName_When_Validated_Then_ShouldHaveError(string productName)
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();
        saleItem.ProductName = productName;
        saleItem.CalculateTotalAmount(); // Recalculate totals after changing product name

        // Act
        var result = _validator.TestValidate(saleItem);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProductName);
    }

    /// <summary>
    /// Tests that validation fails when product name exceeds maximum length.
    /// </summary>
    [Fact(DisplayName = "Product name longer than maximum length should fail validation")]
    public void Given_ProductNameLongerThanMaximum_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();
        saleItem.ProductName = new string('A', 101); // 101 characters
        saleItem.CalculateTotalAmount(); // Recalculate totals after changing product name

        // Act
        var result = _validator.TestValidate(saleItem);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProductName);
    }

    /// <summary>
    /// Tests that validation fails for invalid product code formats.
    /// </summary>
    /// <param name="productCode">The invalid product code to test.</param>
    [Theory(DisplayName = "Invalid product code formats should fail validation")]
    [InlineData("")] // Empty
    [InlineData("INVALID")] // Wrong format
    [InlineData("PROD-")] // Incomplete format
    [InlineData("PROD-123")] // Too short
    [InlineData("PROD-12345")] // Too long
    [InlineData("PROD-ABC1")] // Contains letters
    public void Given_InvalidProductCode_When_Validated_Then_ShouldHaveError(string productCode)
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();
        saleItem.ProductCode = productCode;
        saleItem.CalculateTotalAmount(); // Recalculate totals after changing product code

        // Act
        var result = _validator.TestValidate(saleItem);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProductCode);
    }

    /// <summary>
    /// Tests that validation fails when product description exceeds maximum length.
    /// </summary>
    [Fact(DisplayName = "Product description longer than maximum length should fail validation")]
    public void Given_ProductDescriptionLongerThanMaximum_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();
        saleItem.ProductDescription = new string('A', 501); // 501 characters
        saleItem.CalculateTotalAmount(); // Recalculate totals after changing product description

        // Act
        var result = _validator.TestValidate(saleItem);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProductDescription);
    }

    /// <summary>
    /// Tests that validation fails for invalid quantity values.
    /// </summary>
    /// <param name="quantity">The invalid quantity to test.</param>
    [Theory(DisplayName = "Invalid quantity values should fail validation")]
    [InlineData(0)] // Zero
    [InlineData(-1)] // Negative
    [InlineData(-5)] // Negative
    [InlineData(21)] // Too high
    [InlineData(25)] // Too high
    [InlineData(100)] // Too high
    public void Given_InvalidQuantity_When_Validated_Then_ShouldHaveError(int quantity)
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();
        saleItem.Quantity = quantity;
        saleItem.CalculateTotalAmount(); // Recalculate totals after changing quantity

        // Act
        var result = _validator.TestValidate(saleItem);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    /// <summary>
    /// Tests that validation fails when unit price is negative.
    /// </summary>
    [Fact(DisplayName = "Negative unit price should fail validation")]
    public void Given_NegativeUnitPrice_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();
        saleItem.UnitPrice = -10.00m;
        saleItem.CalculateTotalAmount(); // Recalculate totals after changing unit price

        // Act
        var result = _validator.TestValidate(saleItem);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UnitPrice);
    }

    /// <summary>
    /// Tests that validation fails when sale item status is Unknown.
    /// </summary>
    [Fact(DisplayName = "Unknown sale item status should fail validation")]
    public void Given_UnknownSaleItemStatus_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();
        saleItem.Status = SaleItemStatus.Unknown;
        saleItem.CalculateTotalAmount(); // Recalculate totals after changing status

        // Act
        var result = _validator.TestValidate(saleItem);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    /// <summary>
    /// Tests that validation passes for valid sale item statuses.
    /// </summary>
    /// <param name="status">The valid sale item status to test.</param>
    [Theory(DisplayName = "Valid sale item statuses should pass validation")]
    [InlineData(SaleItemStatus.Active)]
    [InlineData(SaleItemStatus.Cancelled)]
    public void Given_ValidSaleItemStatus_When_Validated_Then_ShouldNotHaveError(SaleItemStatus status)
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();
        saleItem.Status = status;
        saleItem.CalculateTotalAmount(); // Recalculate totals after changing status

        // Act
        var result = _validator.TestValidate(saleItem);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }

    /// <summary>
    /// Tests that validation passes for zero unit price.
    /// </summary>
    [Fact(DisplayName = "Zero unit price should pass validation")]
    public void Given_ZeroUnitPrice_When_Validated_Then_ShouldNotHaveError()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();
        saleItem.UnitPrice = 0.00m;
        saleItem.CalculateTotalAmount(); // Recalculate totals after changing unit price

        // Act
        var result = _validator.TestValidate(saleItem);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.UnitPrice);
    }

    /// <summary>
    /// Tests that validation passes for zero discount percentage.
    /// </summary>
    [Fact(DisplayName = "Zero discount percentage should pass validation")]
    public void Given_ZeroDiscountPercentage_When_Validated_Then_ShouldNotHaveError()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();
        saleItem.DiscountPercentage = 0m;
        saleItem.CalculateTotalAmount(); // Recalculate totals after changing discount percentage

        // Act
        var result = _validator.TestValidate(saleItem);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.DiscountPercentage);
    }

    /// <summary>
    /// Tests that validation passes for 100% discount percentage.
    /// </summary>
    [Fact(DisplayName = "100% discount percentage should pass validation")]
    public void Given_100PercentDiscountPercentage_When_Validated_Then_ShouldNotHaveError()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();
        saleItem.DiscountPercentage = 100m;
        saleItem.CalculateTotalAmount(); // Recalculate totals after changing discount percentage

        // Act
        var result = _validator.TestValidate(saleItem);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.DiscountPercentage);
    }

    /// <summary>
    /// Tests that validation passes for zero discount amount.
    /// </summary>
    [Fact(DisplayName = "Zero discount amount should pass validation")]
    public void Given_ZeroDiscountAmount_When_Validated_Then_ShouldNotHaveError()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();
        saleItem.DiscountAmount = 0.00m;
        saleItem.CalculateTotalAmount(); // Recalculate totals after changing discount amount

        // Act
        var result = _validator.TestValidate(saleItem);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.DiscountAmount);
    }

    /// <summary>
    /// Tests that validation passes for zero total item amount.
    /// </summary>
    [Fact(DisplayName = "Zero total item amount should pass validation")]
    public void Given_ZeroTotalItemAmount_When_Validated_Then_ShouldNotHaveError()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();
        saleItem.UnitPrice = 0.00m;
        saleItem.Quantity = 1;
        saleItem.DiscountPercentage = 0m;
        saleItem.DiscountAmount = 0.00m;
        saleItem.TotalItemAmount = 0.00m;

        // Act
        var result = _validator.TestValidate(saleItem);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.TotalItemAmount);
    }

    /// <summary>
    /// Tests that validation passes for valid quantity values.
    /// </summary>
    /// <param name="quantity">The valid quantity to test.</param>
    [Theory(DisplayName = "Valid quantity values should pass validation")]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(20)]
    public void Given_ValidQuantity_When_Validated_Then_ShouldNotHaveError(int quantity)
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();
        saleItem.Quantity = quantity;
        saleItem.CalculateTotalAmount(); // Recalculate totals after changing quantity

        // Act
        var result = _validator.TestValidate(saleItem);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
    }

    /// <summary>
    /// Tests that validation fails when total item amount doesn't match the calculated value.
    /// </summary>
    [Fact(DisplayName = "Incorrect total item amount should fail validation")]
    public void Given_IncorrectTotalItemAmount_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();
        saleItem.UnitPrice = 10.00m;
        saleItem.Quantity = 5;
        saleItem.DiscountPercentage = 10m; // 10% discount for quantity 5
        saleItem.DiscountAmount = 5.00m;
        saleItem.TotalItemAmount = 100.00m; // Should be 45.00m (10 * 5 - 5)

        // Act
        var result = _validator.TestValidate(saleItem);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TotalItemAmount);
    }

    /// <summary>
    /// Tests that validation fails when discount amount doesn't match the calculated value.
    /// </summary>
    [Fact(DisplayName = "Incorrect discount amount should fail validation")]
    public void Given_IncorrectDiscountAmount_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();
        saleItem.UnitPrice = 20.00m;
        saleItem.Quantity = 10;
        saleItem.DiscountPercentage = 20m;
        saleItem.DiscountAmount = 100.00m; // Should be 40.00m (20 * 10 * 20 / 100)

        // Act
        var result = _validator.TestValidate(saleItem);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DiscountAmount);
    }

    /// <summary>
    /// Tests that validation passes when all calculated values are correct.
    /// </summary>
    [Fact(DisplayName = "Correct calculated values should pass validation")]
    public void Given_CorrectCalculatedValues_When_Validated_Then_ShouldNotHaveError()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();
        saleItem.UnitPrice = 15.00m;
        saleItem.Quantity = 4;
        saleItem.DiscountPercentage = 10m;
        saleItem.DiscountAmount = 6.00m; // (15 * 4 * 10) / 100 = 6
        saleItem.TotalItemAmount = 54.00m; // (15 * 4) - 6 = 54

        // Act
        var result = _validator.TestValidate(saleItem);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.TotalItemAmount);
        result.ShouldNotHaveValidationErrorFor(x => x.DiscountAmount);
    }
}
