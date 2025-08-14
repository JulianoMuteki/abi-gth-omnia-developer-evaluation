using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Represents the response returned after successfully retrieving a sale.
/// </summary>
/// <remarks>
/// This response contains the complete sale information including all its properties
/// and related data that can be used for display or further processing.
/// </remarks>
public class GetSaleResult
{
    /// <summary>
    /// Gets or sets the unique identifier of the sale.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique sale number.
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date when the sale was made.
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Gets or sets the customer ID.
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the customer name.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the customer email.
    /// </summary>
    public string CustomerEmail { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the customer phone.
    /// </summary>
    public string CustomerPhone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch ID.
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// Gets or sets the branch name.
    /// </summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch code.
    /// </summary>
    public string BranchCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total sale amount.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets the sale status.
    /// </summary>
    public SaleStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the sale was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time of the last update to the sale's information.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the sale was cancelled (if applicable).
    /// </summary>
    public DateTime? CancelledAt { get; set; }

    /// <summary>
    /// Gets or sets the ID of the user who cancelled the sale (if applicable).
    /// </summary>
    public Guid? CancelledBy { get; set; }

    /// <summary>
    /// Gets or sets the list of sale items.
    /// </summary>
    public List<GetSaleItemResult> Items { get; set; } = new List<GetSaleItemResult>();
}

/// <summary>
/// Represents the response returned after successfully retrieving a sale item.
/// </summary>
public class GetSaleItemResult
{
    /// <summary>
    /// Gets or sets the unique identifier of the sale item.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the sale ID that this item belongs to.
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// Gets or sets the product ID.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the product name.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product code.
    /// </summary>
    public string ProductCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product description.
    /// </summary>
    public string ProductDescription { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity of the product sold.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the product.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the discount percentage applied to this item.
    /// </summary>
    public decimal DiscountPercentage { get; set; }

    /// <summary>
    /// Gets or sets the discount amount in currency.
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Gets or sets the total amount for this item (after discounts).
    /// </summary>
    public decimal TotalItemAmount { get; set; }

    /// <summary>
    /// Gets or sets the item status.
    /// </summary>
    public SaleItemStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the item was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time of the last update to the item's information.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the item was cancelled (if applicable).
    /// </summary>
    public DateTime? CancelledAt { get; set; }

    /// <summary>
    /// Gets or sets the ID of the user who cancelled the item (if applicable).
    /// </summary>
    public Guid? CancelledBy { get; set; }
}
