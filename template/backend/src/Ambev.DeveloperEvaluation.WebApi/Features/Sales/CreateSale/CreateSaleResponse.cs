using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// API response model for CreateSale operation
/// </summary>
public class CreateSaleResponse
{
    /// <summary>
    /// The unique identifier of the created sale
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The unique sale number
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// The date when the sale was made
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// The customer ID
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// The customer name
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// The customer email
    /// </summary>
    public string CustomerEmail { get; set; } = string.Empty;

    /// <summary>
    /// The customer phone
    /// </summary>
    public string CustomerPhone { get; set; } = string.Empty;

    /// <summary>
    /// The branch ID where the sale was made
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// The branch name
    /// </summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// The branch code
    /// </summary>
    public string BranchCode { get; set; } = string.Empty;

    /// <summary>
    /// The total sale amount
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// The sale's current status
    /// </summary>
    public SaleStatus Status { get; set; }

    /// <summary>
    /// The date and time when the sale was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The list of sale items
    /// </summary>
    public List<CreateSaleItemResponse> Items { get; set; } = new List<CreateSaleItemResponse>();
}

/// <summary>
/// API response model for CreateSaleItem operation
/// </summary>
public class CreateSaleItemResponse
{
    /// <summary>
    /// The unique identifier of the created sale item
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The sale ID that this item belongs to
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// The product ID
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// The product name
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// The product code
    /// </summary>
    public string ProductCode { get; set; } = string.Empty;

    /// <summary>
    /// The product description
    /// </summary>
    public string ProductDescription { get; set; } = string.Empty;

    /// <summary>
    /// The quantity of the product sold
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// The unit price of the product
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// The discount percentage applied to this item
    /// </summary>
    public decimal DiscountPercentage { get; set; }

    /// <summary>
    /// The discount amount in currency
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// The total amount for this item (after discounts)
    /// </summary>
    public decimal TotalItemAmount { get; set; }

    /// <summary>
    /// The item's current status
    /// </summary>
    public SaleItemStatus Status { get; set; }

    /// <summary>
    /// The date and time when the item was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
