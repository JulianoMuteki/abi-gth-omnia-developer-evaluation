using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a branch/store location in the system.
/// </summary>
public class Branch : BaseEntity
{
    /// <summary>
    /// Gets the branch name.
    /// Must not be null or empty and should be descriptive.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets the branch code.
    /// Must be unique and is used as a business identifier.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets the branch address.
    /// Contains the full address information of the branch.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Gets the branch phone number.
    /// Must be a valid phone number format.
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Gets the branch email address.
    /// Must be a valid email format.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets the branch's current status.
    /// Indicates whether the branch is active or inactive in the system.
    /// </summary>
    public BranchStatus Status { get; set; }

    /// <summary>
    /// Gets the date and time when the branch was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets the date and time of the last update to the branch's information.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Initializes a new instance of the Branch class.
    /// </summary>
    public Branch()
    {
        CreatedAt = DateTime.UtcNow;
        Status = BranchStatus.Active;
    }

    /// <summary>
    /// Activates the branch.
    /// Changes the branch's status to Active.
    /// </summary>
    public void Activate()
    {
        Status = BranchStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the branch.
    /// Changes the branch's status to Inactive.
    /// </summary>
    public void Deactivate()
    {
        Status = BranchStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the branch information.
    /// </summary>
    /// <param name="name">The new branch name.</param>
    /// <param name="code">The new branch code.</param>
    /// <param name="address">The new branch address.</param>
    /// <param name="phone">The new branch phone.</param>
    /// <param name="email">The new branch email.</param>
    public void Update(string name, string code, string address, string phone, string email)
    {
        Name = name;
        Code = code;
        Address = address;
        Phone = phone;
        Email = email;
        UpdatedAt = DateTime.UtcNow;
    }
}
