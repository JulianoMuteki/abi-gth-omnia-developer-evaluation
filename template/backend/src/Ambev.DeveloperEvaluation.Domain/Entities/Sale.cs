using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a sale transaction in the system.
/// </summary>
public class Sale : BaseEntity
{
    /// <summary>
    /// Gets the unique sale number.
    /// Must be unique and is used as a business identifier.
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets the date when the sale was made.
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Gets the customer ID (External Identity pattern).
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Gets the customer name
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets the customer email
    /// </summary>
    public string CustomerEmail { get; set; } = string.Empty;

    /// <summary>
    /// Gets the customer phone
    /// </summary>
    public string CustomerPhone { get; set; } = string.Empty;

    /// <summary>
    /// Gets the branch ID where the sale was made.
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// Gets the branch name
    /// </summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// Gets the branch code
    /// </summary>
    public string BranchCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets the total sale amount.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets the sale's current status.
    /// Indicates whether the sale is active or cancelled.
    /// </summary>
    public SaleStatus Status { get; set; }

    /// <summary>
    /// Gets the date and time when the sale was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets the date and time of the last update to the sale's information.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets the date and time when the sale was cancelled (if applicable).
    /// </summary>
    public DateTime? CancelledAt { get; set; }

    /// <summary>
    /// Gets the ID of the user who cancelled the sale (if applicable).
    /// </summary>
    public Guid? CancelledBy { get; set; }

    /// <summary>
    /// Gets the collection of sale items.
    /// </summary>
    public virtual ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();

    /// <summary>
    /// Navigation property for the branch where the sale was made.
    /// </summary>
    public virtual Branch? Branch { get; set; }

    /// <summary>
    /// Initializes a new instance of the Sale class.
    /// </summary>
    public Sale()
    {
        CreatedAt = DateTime.UtcNow;
        SaleDate = DateTime.UtcNow;
        Status = SaleStatus.Active;
    }

    /// <summary>
    /// Adds an item to the sale.
    /// </summary>
    /// <param name="item">The sale item to add.</param>
    public void AddItem(SaleItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        if (Status == SaleStatus.Cancelled)
            throw new InvalidOperationException("Cannot add items to a cancelled sale.");

        item.SaleId = Id;
        Items.Add(item);
        CalculateTotalAmount();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Removes an item from the sale.
    /// </summary>
    /// <param name="itemId">The ID of the item to remove.</param>
    public void RemoveItem(Guid itemId)
    {
        if (Status == SaleStatus.Cancelled)
            throw new InvalidOperationException("Cannot remove items from a cancelled sale.");

        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            Items.Remove(item);
            CalculateTotalAmount();
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Cancels an item in the sale.
    /// </summary>
    /// <param name="itemId">The ID of the item to cancel.</param>
    /// <param name="cancelledBy">The ID of the user who cancelled the item.</param>
    public void CancelItem(Guid itemId, Guid cancelledBy)
    {
        if (Status == SaleStatus.Cancelled)
            throw new InvalidOperationException("Cannot cancel items in a cancelled sale.");

        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            item.Cancel(cancelledBy);
            CalculateTotalAmount();
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Cancels the entire sale.
    /// </summary>
    /// <param name="cancelledBy">The ID of the user who cancelled the sale.</param>
    public void Cancel(Guid cancelledBy)
    {
        if (Status == SaleStatus.Cancelled)
            throw new InvalidOperationException("Sale is already cancelled.");

        Status = SaleStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        CancelledBy = cancelledBy;
        UpdatedAt = DateTime.UtcNow;

        // Cancel all active items
        foreach (var item in Items.Where(i => i.Status == SaleItemStatus.Active))
        {
            item.Cancel(cancelledBy);
        }
    }

    /// <summary>
    /// Calculates the total amount of the sale based on all active items.
    /// </summary>
    public void CalculateTotalAmount()
    {
        TotalAmount = Items
            .Where(i => i.Status == SaleItemStatus.Active)
            .Sum(i => i.TotalItemAmount);
    }

    /// <summary>
    /// Updates the customer information.
    /// </summary>
    /// <param name="customerName">The customer name.</param>
    /// <param name="customerEmail">The customer email.</param>
    /// <param name="customerPhone">The customer phone.</param>
    public void UpdateCustomerInfo(string customerName, string customerEmail, string customerPhone)
    {
        CustomerName = customerName;
        CustomerEmail = customerEmail;
        CustomerPhone = customerPhone;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the branch information.
    /// </summary>
    /// <param name="branchName">The branch name.</param>
    /// <param name="branchCode">The branch code.</param>
    public void UpdateBranchInfo(string branchName, string branchCode)
    {
        BranchName = branchName;
        BranchCode = branchCode;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the total number of items in the sale.
    /// </summary>
    /// <returns>The total number of active items.</returns>
    public int GetTotalItemsCount()
    {
        return Items.Count(i => i.Status == SaleItemStatus.Active);
    }

    /// <summary>
    /// Gets the total discount amount for the sale.
    /// </summary>
    /// <returns>The total discount amount.</returns>
    public decimal GetTotalDiscountAmount()
    {
        return Items
            .Where(i => i.Status == SaleItemStatus.Active)
            .Sum(i => i.DiscountAmount);
    }
}
