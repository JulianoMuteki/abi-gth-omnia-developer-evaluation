using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Specifications;
using Ambev.DeveloperEvaluation.Unit.Domain.Specifications.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Specifications;

/// <summary>
/// Contains unit tests for the ActiveSaleItemSpecification class.
/// Tests cover the specification logic for determining if a sale item is active.
/// </summary>
public class ActiveSaleItemSpecificationTests
{
    /// <summary>
    /// Tests that the specification correctly validates sale item status.
    /// This test verifies that:
    /// - Active sale items return true
    /// - Cancelled sale items return false
    /// - Unknown sale items return false
    /// </summary>
    /// <param name="status">The sale item status to test.</param>
    /// <param name="expectedResult">The expected result from the specification.</param>
    [Theory(DisplayName = "IsSatisfiedBy should validate sale item status correctly")]
    [InlineData(SaleItemStatus.Active, true)]
    [InlineData(SaleItemStatus.Cancelled, false)]
    [InlineData(SaleItemStatus.Unknown, false)]
    public void IsSatisfiedBy_ShouldValidateSaleItemStatus(SaleItemStatus status, bool expectedResult)
    {
        // Arrange
        var saleItem = ActiveSaleItemSpecificationTestData.GenerateSaleItem(status);
        var specification = new ActiveSaleItemSpecification();

        // Act
        var result = specification.IsSatisfiedBy(saleItem);

        // Assert
        result.Should().Be(expectedResult);
    }
}
