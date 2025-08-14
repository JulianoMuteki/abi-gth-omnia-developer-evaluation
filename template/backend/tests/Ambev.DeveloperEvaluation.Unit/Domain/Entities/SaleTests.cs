using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Contains unit tests for the Sale entity class.
/// Tests cover status changes, item management, and business logic scenarios.
/// </summary>
public class SaleTests
{
    /// <summary>
    /// Tests that when an active sale is cancelled, its status changes to Cancelled.
    /// </summary>
    [Fact(DisplayName = "Sale status should change to Cancelled when cancelled")]
    public void Given_ActiveSale_When_Cancelled_Then_StatusShouldBeCancelled()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Status = SaleStatus.Active;
        var cancelledBy = Guid.NewGuid();

        // Act
        sale.Cancel(cancelledBy);

        // Assert
        Assert.Equal(SaleStatus.Cancelled, sale.Status);
        Assert.NotNull(sale.CancelledAt);
        Assert.Equal(cancelledBy, sale.CancelledBy);
        Assert.NotNull(sale.UpdatedAt);
    }

    /// <summary>
    /// Tests that cancelling an already cancelled sale throws an exception.
    /// </summary>
    [Fact(DisplayName = "Cancelling an already cancelled sale should throw exception")]
    public void Given_CancelledSale_When_CancelledAgain_Then_ShouldThrowException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Status = SaleStatus.Cancelled;
        var cancelledBy = Guid.NewGuid();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => sale.Cancel(cancelledBy));
        Assert.Equal("Sale is already cancelled.", exception.Message);
    }

    /// <summary>
    /// Tests that adding an item to an active sale increases the items count.
    /// </summary>
    [Fact(DisplayName = "Adding item to active sale should increase items count")]
    public void Given_ActiveSale_When_ItemAdded_Then_ItemsCountShouldIncrease()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Status = SaleStatus.Active;
        var initialCount = sale.Items.Count;
        var item = SaleItemTestData.GenerateValidSaleItem();

        // Act
        sale.AddItem(item);

        // Assert
        Assert.Equal(initialCount + 1, sale.Items.Count);
        Assert.Contains(item, sale.Items);
        Assert.NotNull(sale.UpdatedAt);
    }

    /// <summary>
    /// Tests that adding an item to a cancelled sale throws an exception.
    /// </summary>
    [Fact(DisplayName = "Adding item to cancelled sale should throw exception")]
    public void Given_CancelledSale_When_ItemAdded_Then_ShouldThrowException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Status = SaleStatus.Cancelled;
        var item = SaleItemTestData.GenerateValidSaleItem();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => sale.AddItem(item));
        Assert.Equal("Cannot add items to a cancelled sale.", exception.Message);
    }

    /// <summary>
    /// Tests that adding a null item throws an exception.
    /// </summary>
    [Fact(DisplayName = "Adding null item should throw exception")]
    public void Given_ActiveSale_When_NullItemAdded_Then_ShouldThrowException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Status = SaleStatus.Active;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => sale.AddItem(null!));
        Assert.Equal("item", exception.ParamName);
    }

    /// <summary>
    /// Tests that removing an item from an active sale decreases the items count.
    /// </summary>
    [Fact(DisplayName = "Removing item from active sale should decrease items count")]
    public void Given_ActiveSaleWithItem_When_ItemRemoved_Then_ItemsCountShouldDecrease()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Status = SaleStatus.Active;
        var item = SaleItemTestData.GenerateValidSaleItem();
        item.Status = SaleItemStatus.Active;
        sale.AddItem(item);

        var initialCount = sale.Items.Count;

        // Act
        sale.RemoveItem(item.Id);

        // Assert
        Assert.Equal(initialCount - 1, sale.Items.Count);
        Assert.DoesNotContain(item, sale.Items);
        Assert.NotNull(sale.UpdatedAt);
    }

    /// <summary>
    /// Tests that removing an item from a cancelled sale throws an exception.
    /// </summary>
    [Fact(DisplayName = "Removing item from cancelled sale should throw exception")]
    public void Given_CancelledSale_When_ItemRemoved_Then_ShouldThrowException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Status = SaleStatus.Cancelled;
        var itemId = Guid.NewGuid();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => sale.RemoveItem(itemId));
        Assert.Equal("Cannot remove items from a cancelled sale.", exception.Message);
    }

    /// <summary>
    /// Tests that cancelling an item in an active sale changes the item status.
    /// </summary>
    [Fact(DisplayName = "Cancelling item in active sale should change item status")]
    public void Given_ActiveSaleWithItem_When_ItemCancelled_Then_ItemStatusShouldBeCancelled()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Status = SaleStatus.Active;
        var item = SaleItemTestData.GenerateValidSaleItem();
        sale.AddItem(item);
        var cancelledBy = Guid.NewGuid();

        // Act
        sale.CancelItem(item.Id, cancelledBy);

        // Assert
        Assert.Equal(SaleItemStatus.Cancelled, item.Status);
        Assert.NotNull(item.CancelledAt);
        Assert.Equal(cancelledBy, item.CancelledBy);
        Assert.NotNull(sale.UpdatedAt);
    }

    /// <summary>
    /// Tests that cancelling an item in a cancelled sale throws an exception.
    /// </summary>
    [Fact(DisplayName = "Cancelling item in cancelled sale should throw exception")]
    public void Given_CancelledSale_When_ItemCancelled_Then_ShouldThrowException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Status = SaleStatus.Cancelled;
        var itemId = Guid.NewGuid();
        var cancelledBy = Guid.NewGuid();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => sale.CancelItem(itemId, cancelledBy));
        Assert.Equal("Cannot cancel items in a cancelled sale.", exception.Message);
    }

    /// <summary>
    /// Tests that calculating total amount includes only active items.
    /// </summary>
    [Fact(DisplayName = "Calculate total amount should include only active items")]
    public void Given_SaleWithActiveAndCancelledItems_When_CalculateTotalAmount_Then_ShouldIncludeOnlyActiveItems()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Status = SaleStatus.Active;
        sale.CalculateTotalAmount();
        var inititalTotalAmount = sale.TotalAmount; // Initial amount

        var activeItem1 = SaleItemTestData.GenerateValidSaleItem();
        activeItem1.UnitPrice = 10.00m;
        activeItem1.Quantity = 2;
        activeItem1.Status = SaleItemStatus.Active;
        activeItem1.CalculateTotalAmount();
        
        var activeItem2 = SaleItemTestData.GenerateValidSaleItem();
        activeItem2.UnitPrice = 15.00m;
        activeItem2.Quantity = 1;
        activeItem2.Status = SaleItemStatus.Active;
        activeItem2.CalculateTotalAmount();
        
        var cancelledItem = SaleItemTestData.GenerateValidSaleItem();
        cancelledItem.UnitPrice = 20.00m;
        cancelledItem.Quantity = 3;
        cancelledItem.Status = SaleItemStatus.Cancelled;
        cancelledItem.CalculateTotalAmount();
        
        sale.AddItem(activeItem1);
        sale.AddItem(activeItem2);
        sale.AddItem(cancelledItem);

        // Act
        sale.CalculateTotalAmount();

        // Assert
        var expectedTotal = activeItem1.TotalItemAmount + activeItem2.TotalItemAmount + inititalTotalAmount;
        Assert.Equal(expectedTotal, sale.TotalAmount);
    }

    /// <summary>
    /// Tests that updating customer information updates the sale properties.
    /// </summary>
    [Fact(DisplayName = "Updating customer info should update sale properties")]
    public void Given_Sale_When_CustomerInfoUpdated_Then_PropertiesShouldBeUpdated()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var newName = "New Customer Name";
        var newEmail = "newcustomer@example.com";
        var newPhone = "+5511999999999";

        // Act
        sale.UpdateCustomerInfo(newName, newEmail, newPhone);

        // Assert
        Assert.Equal(newName, sale.CustomerName);
        Assert.Equal(newEmail, sale.CustomerEmail);
        Assert.Equal(newPhone, sale.CustomerPhone);
        Assert.NotNull(sale.UpdatedAt);
    }

    /// <summary>
    /// Tests that updating branch information updates the sale properties.
    /// </summary>
    [Fact(DisplayName = "Updating branch info should update sale properties")]
    public void Given_Sale_When_BranchInfoUpdated_Then_PropertiesShouldBeUpdated()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var newBranchName = "New Branch Name";
        var newBranchCode = "BR-999";

        // Act
        sale.UpdateBranchInfo(newBranchName, newBranchCode);

        // Assert
        Assert.Equal(newBranchName, sale.BranchName);
        Assert.Equal(newBranchCode, sale.BranchCode);
        Assert.NotNull(sale.UpdatedAt);
    }

    /// <summary>
    /// Tests that getting total items count returns only active items.
    /// </summary>
    [Fact(DisplayName = "Get total items count should return only active items")]
    public void Given_SaleWithActiveAndCancelledItems_When_GetTotalItemsCount_Then_ShouldReturnOnlyActiveItems()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Status = SaleStatus.Active;
        int totalItems = sale.GetTotalItemsCount(); // Initial count

        var activeItem1 = SaleItemTestData.GenerateValidSaleItem();
        activeItem1.Status = SaleItemStatus.Active;
        var activeItem2 = SaleItemTestData.GenerateValidSaleItem();
        activeItem2.Status = SaleItemStatus.Active;
        var cancelledItem = SaleItemTestData.GenerateValidSaleItem();
        cancelledItem.Status = SaleItemStatus.Cancelled;
        
        sale.AddItem(activeItem1);
        sale.AddItem(activeItem2);
        sale.AddItem(cancelledItem);

        // Act
        var totalCount = sale.GetTotalItemsCount();

        // Assert
        Assert.Equal(totalItems + 2, totalCount);
    }

    /// <summary>
    /// Tests that getting total discount amount returns only active items.
    /// </summary>
    [Fact(DisplayName = "Get total discount amount should return only active items")]
    public void Given_SaleWithActiveAndCancelledItems_When_GetTotalDiscountAmount_Then_ShouldReturnOnlyActiveItems()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Status = SaleStatus.Active;
        var inititalTotalDiscount = sale.GetTotalDiscountAmount(); // Initial discount

        var activeItem1 = SaleItemTestData.GenerateValidSaleItem();
        activeItem1.Status = SaleItemStatus.Active;
        activeItem1.DiscountAmount = 5.00m;
        activeItem1.CalculateTotalAmount();
        
        var activeItem2 = SaleItemTestData.GenerateValidSaleItem();
        activeItem2.Status = SaleItemStatus.Active;
        activeItem2.DiscountAmount = 3.00m;
        activeItem2.CalculateTotalAmount();
        
        var cancelledItem = SaleItemTestData.GenerateValidSaleItem();
        cancelledItem.DiscountAmount = 10.00m;
        cancelledItem.Status = SaleItemStatus.Cancelled;
        cancelledItem.CalculateTotalAmount();
        
        sale.AddItem(activeItem1);
        sale.AddItem(activeItem2);
        sale.AddItem(cancelledItem);

        // Act
        var totalDiscount = sale.GetTotalDiscountAmount();

        // Assert
        var expectedDiscount = activeItem1.DiscountAmount + activeItem2.DiscountAmount + inititalTotalDiscount;
        Assert.Equal(expectedDiscount, totalDiscount);
    }

    /// <summary>
    /// Tests that cancelling a sale also cancels all active items.
    /// </summary>
    [Fact(DisplayName = "Cancelling sale should cancel all active items")]
    public void Given_SaleWithActiveItems_When_SaleCancelled_Then_AllActiveItemsShouldBeCancelled()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Status = SaleStatus.Active;
        
        var activeItem1 = SaleItemTestData.GenerateValidSaleItem();
        activeItem1.Status = SaleItemStatus.Active;
        var activeItem2 = SaleItemTestData.GenerateValidSaleItem();
        activeItem2.Status = SaleItemStatus.Active;
        var cancelledItem = SaleItemTestData.GenerateValidSaleItem();
        cancelledItem.Status = SaleItemStatus.Cancelled;
        
        sale.AddItem(activeItem1);
        sale.AddItem(activeItem2);
        sale.AddItem(cancelledItem);
        
        var cancelledBy = Guid.NewGuid();

        // Act
        sale.Cancel(cancelledBy);

        // Assert
        Assert.Equal(SaleStatus.Cancelled, sale.Status);
        Assert.Equal(SaleItemStatus.Cancelled, activeItem1.Status);
        Assert.Equal(SaleItemStatus.Cancelled, activeItem2.Status);
        Assert.Equal(SaleItemStatus.Cancelled, cancelledItem.Status); // Already cancelled
        Assert.NotNull(activeItem1.CancelledAt);
        Assert.NotNull(activeItem2.CancelledAt);
        Assert.Equal(cancelledBy, activeItem1.CancelledBy);
        Assert.Equal(cancelledBy, activeItem2.CancelledBy);
    }
}
