using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents an item within a sale transaction.
/// </summary>
public class SaleItem : BaseEntity
{
    /// <summary>
    /// Gets the sale ID that this item belongs to.
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// Gets the product ID (External Identity pattern).
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets the product name
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Gets the product code
    /// </summary>
    public string ProductCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets the product description
    /// </summary>
    public string ProductDescription { get; set; } = string.Empty;

    /// <summary>
    /// Gets the quantity of the product sold.
    /// Must be between 1 and 20 according to business rules.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets the unit price of the product.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets the discount percentage applied to this item.
    /// Calculated based on quantity according to business rules.
    /// </summary>
    public decimal DiscountPercentage { get; set; }

    /// <summary>
    /// Gets the discount amount in currency.
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Gets the total amount for this item (after discounts).
    /// </summary>
    public decimal TotalItemAmount { get; set; }

    /// <summary>
    /// Gets the item's current status.
    /// Indicates whether the item is active or cancelled.
    /// </summary>
    public SaleItemStatus Status { get; set; }

    /// <summary>
    /// Gets the date and time when the item was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets the date and time of the last update to the item's information.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets the date and time when the item was cancelled (if applicable).
    /// </summary>
    public DateTime? CancelledAt { get; set; }

    /// <summary>
    /// Gets the ID of the user who cancelled the item (if applicable).
    /// </summary>
    public Guid? CancelledBy { get; set; }

    /// <summary>
    /// Navigation property for the sale that this item belongs to.
    /// </summary>
    public virtual Sale? Sale { get; set; }

    /// <summary>
    /// Initializes a new instance of the SaleItem class.
    /// </summary>
    public SaleItem()
    {
        CreatedAt = DateTime.UtcNow;
        Status = SaleItemStatus.Active;
    }

    /// <summary>
    /// Calculates the discount based on quantity according to business rules.
    /// </summary>
    /// <returns>The discount percentage to be applied.</returns>
    public decimal CalculateDiscountPercentage()
    {
        if (Quantity < 4)
            return 0;

        if (Quantity >= 10 && Quantity <= 20)
            return 20;

        if (Quantity >= 4)
            return 10;

        return 0;
    }

    /// <summary>
    /// Calculates the total amount for this item including discounts.
    /// </summary>
    public void CalculateTotalAmount()
    {
        DiscountPercentage = CalculateDiscountPercentage();
        DiscountAmount = (UnitPrice * Quantity * DiscountPercentage) / 100;
        TotalItemAmount = (UnitPrice * Quantity) - DiscountAmount;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancels this sale item.
    /// </summary>
    /// <param name="cancelledBy">The ID of the user who cancelled the item.</param>
    public void Cancel(Guid cancelledBy)
    {
        Status = SaleItemStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        CancelledBy = cancelledBy;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the item quantity and recalculates totals.
    /// </summary>
    /// <param name="quantity">The new quantity.</param>
    public void UpdateQuantity(int quantity)
    {
        if (quantity <= 0 || quantity > 20)
            throw new ArgumentException("Quantity must be between 1 and 20.");

        Quantity = quantity;
        CalculateTotalAmount();
    }

    /// <summary>
    /// Updates the unit price and recalculates totals.
    /// </summary>
    /// <param name="unitPrice">The new unit price.</param>
    public void UpdateUnitPrice(decimal unitPrice)
    {
        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative.");

        UnitPrice = unitPrice;
        CalculateTotalAmount();
    }
}
