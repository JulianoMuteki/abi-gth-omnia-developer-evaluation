using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Contains unit tests for the SaleItem entity class.
/// Tests cover status changes, calculations, and business logic scenarios.
/// </summary>
public class SaleItemTests
{
    /// <summary>
    /// Tests that when an active item is cancelled, its status changes to Cancelled.
    /// </summary>
    [Fact(DisplayName = "SaleItem status should change to Cancelled when cancelled")]
    public void Given_ActiveSaleItem_When_Cancelled_Then_StatusShouldBeCancelled()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();
        saleItem.Status = SaleItemStatus.Active;
        var cancelledBy = Guid.NewGuid();

        // Act
        saleItem.Cancel(cancelledBy);

        // Assert
        Assert.Equal(SaleItemStatus.Cancelled, saleItem.Status);
        Assert.NotNull(saleItem.CancelledAt);
        Assert.Equal(cancelledBy, saleItem.CancelledBy);
        Assert.NotNull(saleItem.UpdatedAt);
    }

    /// <summary>
    /// Tests that calculating discount percentage returns 0% for quantities less than 4.
    /// </summary>
    [Fact(DisplayName = "Discount percentage should be 0% for quantities less than 4")]
    public void Given_QuantityLessThan4_When_CalculateDiscountPercentage_Then_ShouldReturn0()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();
        saleItem.Quantity = SaleItemTestData.GenerateQuantityWithNoDiscount();

        // Act
        var discountPercentage = saleItem.CalculateDiscountPercentage();

        // Assert
        Assert.Equal(0m, discountPercentage);
    }

    /// <summary>
    /// Tests that calculating discount percentage returns 10% for quantities between 4 and 9.
    /// </summary>
    [Fact(DisplayName = "Discount percentage should be 10% for quantities between 4 and 9")]
    public void Given_QuantityBetween4And9_When_CalculateDiscountPercentage_Then_ShouldReturn10()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();
        saleItem.Quantity = SaleItemTestData.GenerateQuantityFor10PercentDiscount();

        // Act
        var discountPercentage = saleItem.CalculateDiscountPercentage();

        // Assert
        Assert.Equal(10m, discountPercentage);
    }

    /// <summary>
    /// Tests that calculating discount percentage returns 20% for quantities between 10 and 20.
    /// </summary>
    [Fact(DisplayName = "Discount percentage should be 20% for quantities between 10 and 20")]
    public void Given_QuantityBetween10And20_When_CalculateDiscountPercentage_Then_ShouldReturn20()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();
        saleItem.Quantity = SaleItemTestData.GenerateQuantityFor20PercentDiscount();

        // Act
        var discountPercentage = saleItem.CalculateDiscountPercentage();

        // Assert
        Assert.Equal(20m, discountPercentage);
    }

    /// <summary>
    /// Tests that calculating total amount correctly calculates discount and total.
    /// </summary>
    [Fact(DisplayName = "Calculate total amount should correctly calculate discount and total")]
    public void Given_SaleItemWithValidData_When_CalculateTotalAmount_Then_ShouldCalculateCorrectly()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();
        saleItem.UnitPrice = 10.00m;
        saleItem.Quantity = 5; // 10% discount

        // Act
        saleItem.CalculateTotalAmount();

        // Assert
        Assert.Equal(10m, saleItem.DiscountPercentage);
        Assert.Equal(5.00m, saleItem.DiscountAmount); // (10 * 5 * 10) / 100 = 5
        Assert.Equal(45.00m, saleItem.TotalItemAmount); // (10 * 5) - 5 = 45
        Assert.NotNull(saleItem.UpdatedAt);
    }

    /// <summary>
    /// Tests that calculating total amount with 20% discount works correctly.
    /// </summary>
    [Fact(DisplayName = "Calculate total amount with 20% discount should work correctly")]
    public void Given_SaleItemWith20PercentDiscount_When_CalculateTotalAmount_Then_ShouldCalculateCorrectly()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();
        saleItem.UnitPrice = 20.00m;
        saleItem.Quantity = 10; // 20% discount

        // Act
        saleItem.CalculateTotalAmount();

        // Assert
        Assert.Equal(20m, saleItem.DiscountPercentage);
        Assert.Equal(40.00m, saleItem.DiscountAmount); // (20 * 10 * 20) / 100 = 40
        Assert.Equal(160.00m, saleItem.TotalItemAmount); // (20 * 10) - 40 = 160
    }

    /// <summary>
    /// Tests that calculating total amount with no discount works correctly.
    /// </summary>
    [Fact(DisplayName = "Calculate total amount with no discount should work correctly")]
    public void Given_SaleItemWithNoDiscount_When_CalculateTotalAmount_Then_ShouldCalculateCorrectly()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();
        saleItem.UnitPrice = 15.00m;
        saleItem.Quantity = 2; // No discount

        // Act
        saleItem.CalculateTotalAmount();

        // Assert
        Assert.Equal(0m, saleItem.DiscountPercentage);
        Assert.Equal(0.00m, saleItem.DiscountAmount);
        Assert.Equal(30.00m, saleItem.TotalItemAmount); // 15 * 2 = 30
    }

    /// <summary>
    /// Tests that updating quantity with valid value updates the item and recalculates totals.
    /// </summary>
    [Fact(DisplayName = "Updating quantity with valid value should update item and recalculate totals")]
    public void Given_SaleItem_When_QuantityUpdatedWithValidValue_Then_ShouldUpdateAndRecalculate()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();
        saleItem.UnitPrice = 10.00m;
        saleItem.Quantity = 1;
        saleItem.CalculateTotalAmount();
        var newQuantity = 5;

        // Act
        saleItem.UpdateQuantity(newQuantity);

        // Assert
        Assert.Equal(newQuantity, saleItem.Quantity);
        Assert.Equal(10m, saleItem.DiscountPercentage); // 10% discount for quantity 5
        Assert.Equal(5.00m, saleItem.DiscountAmount); // (10 * 5 * 10) / 100 = 5
        Assert.Equal(45.00m, saleItem.TotalItemAmount); // (10 * 5) - 5 = 45
        Assert.NotNull(saleItem.UpdatedAt);
    }

    /// <summary>
    /// Tests that updating quantity with zero throws an exception.
    /// </summary>
    [Fact(DisplayName = "Updating quantity with zero should throw exception")]
    public void Given_SaleItem_When_QuantityUpdatedWithZero_Then_ShouldThrowException()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => saleItem.UpdateQuantity(0));
        Assert.Equal("Quantity must be between 1 and 20.", exception.Message);
    }

    /// <summary>
    /// Tests that updating quantity with negative value throws an exception.
    /// </summary>
    [Fact(DisplayName = "Updating quantity with negative value should throw exception")]
    public void Given_SaleItem_When_QuantityUpdatedWithNegativeValue_Then_ShouldThrowException()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => saleItem.UpdateQuantity(-5));
        Assert.Equal("Quantity must be between 1 and 20.", exception.Message);
    }

    /// <summary>
    /// Tests that updating quantity with value greater than 20 throws an exception.
    /// </summary>
    [Fact(DisplayName = "Updating quantity with value greater than 20 should throw exception")]
    public void Given_SaleItem_When_QuantityUpdatedWithValueGreaterThan20_Then_ShouldThrowException()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => saleItem.UpdateQuantity(25));
        Assert.Equal("Quantity must be between 1 and 20.", exception.Message);
    }

    /// <summary>
    /// Tests that updating unit price with valid value updates the item and recalculates totals.
    /// </summary>
    [Fact(DisplayName = "Updating unit price with valid value should update item and recalculate totals")]
    public void Given_SaleItem_When_UnitPriceUpdatedWithValidValue_Then_ShouldUpdateAndRecalculate()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();
        saleItem.UnitPrice = 10.00m;
        saleItem.Quantity = 5;
        saleItem.CalculateTotalAmount();
        var newUnitPrice = 20.00m;

        // Act
        saleItem.UpdateUnitPrice(newUnitPrice);

        // Assert
        Assert.Equal(newUnitPrice, saleItem.UnitPrice);
        Assert.Equal(10m, saleItem.DiscountPercentage); // 10% discount for quantity 5
        Assert.Equal(10.00m, saleItem.DiscountAmount); // (20 * 5 * 10) / 100 = 10
        Assert.Equal(90.00m, saleItem.TotalItemAmount); // (20 * 5) - 10 = 90
        Assert.NotNull(saleItem.UpdatedAt);
    }

    /// <summary>
    /// Tests that updating unit price with negative value throws an exception.
    /// </summary>
    [Fact(DisplayName = "Updating unit price with negative value should throw exception")]
    public void Given_SaleItem_When_UnitPriceUpdatedWithNegativeValue_Then_ShouldThrowException()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => saleItem.UpdateUnitPrice(-10.00m));
        Assert.Equal("Unit price cannot be negative.", exception.Message);
    }

    /// <summary>
    /// Tests that updating unit price with zero is allowed.
    /// </summary>
    [Fact(DisplayName = "Updating unit price with zero should be allowed")]
    public void Given_SaleItem_When_UnitPriceUpdatedWithZero_Then_ShouldUpdateAndRecalculate()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();
        saleItem.UnitPrice = 10.00m;
        saleItem.Quantity = 5;
        saleItem.CalculateTotalAmount();

        // Act
        saleItem.UpdateUnitPrice(0.00m);

        // Assert
        Assert.Equal(0.00m, saleItem.UnitPrice);
        Assert.Equal(10m, saleItem.DiscountPercentage);
        Assert.Equal(0.00m, saleItem.DiscountAmount); // (0 * 5 * 10) / 100 = 0
        Assert.Equal(0.00m, saleItem.TotalItemAmount); // (0 * 5) - 0 = 0
        Assert.NotNull(saleItem.UpdatedAt);
    }

    /// <summary>
    /// Tests that the constructor initializes the item with correct default values.
    /// </summary>
    [Fact(DisplayName = "Constructor should initialize with correct default values")]
    public void Given_NewSaleItem_When_Created_Then_ShouldHaveCorrectDefaultValues()
    {
        // Act
        var saleItem = new SaleItem();

        // Assert
        Assert.Equal(SaleItemStatus.Active, saleItem.Status);
        Assert.NotNull(saleItem.CreatedAt);
        Assert.Null(saleItem.UpdatedAt);
        Assert.Null(saleItem.CancelledAt);
        Assert.Null(saleItem.CancelledBy);
        Assert.Equal(0, saleItem.Quantity);
        Assert.Equal(0m, saleItem.UnitPrice);
        Assert.Equal(0m, saleItem.DiscountPercentage);
        Assert.Equal(0m, saleItem.DiscountAmount);
        Assert.Equal(0m, saleItem.TotalItemAmount);
    }

    /// <summary>
    /// Tests that calculating total amount with decimal precision works correctly.
    /// </summary>
    [Fact(DisplayName = "Calculate total amount with decimal precision should work correctly")]
    public void Given_SaleItemWithDecimalPrecision_When_CalculateTotalAmount_Then_ShouldCalculateCorrectly()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();
        saleItem.UnitPrice = 19.99m;
        saleItem.Quantity = 3; // No discount

        // Act
        saleItem.CalculateTotalAmount();

        // Assert
        Assert.Equal(0m, saleItem.DiscountPercentage);
        Assert.Equal(0.00m, saleItem.DiscountAmount);
        Assert.Equal(59.97m, saleItem.TotalItemAmount); // 19.99 * 3 = 59.97
    }

    /// <summary>
    /// Tests that calculating total amount with discount and decimal precision works correctly.
    /// </summary>
    [Fact(DisplayName = "Calculate total amount with discount and decimal precision should work correctly")]
    public void Given_SaleItemWithDiscountAndDecimalPrecision_When_CalculateTotalAmount_Then_ShouldCalculateCorrectly()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();
        saleItem.UnitPrice = 33.33m;
        saleItem.Quantity = 6; // 10% discount

        // Act
        saleItem.CalculateTotalAmount();

        // Assert
        Assert.Equal(10m, saleItem.DiscountPercentage);
        Assert.Equal(19.998m, saleItem.DiscountAmount); // (33.33 * 6 * 10) / 100 = 19.998
        Assert.Equal(179.982m, saleItem.TotalItemAmount); // (33.33 * 6) - 19.998 = 179.982
    }
}
