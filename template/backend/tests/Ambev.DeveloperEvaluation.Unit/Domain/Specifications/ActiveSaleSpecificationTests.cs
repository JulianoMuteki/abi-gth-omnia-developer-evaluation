using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Specifications;
using Ambev.DeveloperEvaluation.Unit.Domain.Specifications.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Specifications;

/// <summary>
/// Contains unit tests for the ActiveSaleSpecification class.
/// Tests cover the specification logic for determining if a sale is active.
/// </summary>
public class ActiveSaleSpecificationTests
{
    /// <summary>
    /// Tests that the specification correctly validates sale status.
    /// This test verifies that:
    /// - Active sales return true
    /// - Cancelled sales return false
    /// - Unknown sales return false
    /// </summary>
    /// <param name="status">The sale status to test.</param>
    /// <param name="expectedResult">The expected result from the specification.</param>
    [Theory(DisplayName = "IsSatisfiedBy should validate sale status correctly")]
    [InlineData(SaleStatus.Active, true)]
    [InlineData(SaleStatus.Cancelled, false)]
    [InlineData(SaleStatus.Unknown, false)]
    public void IsSatisfiedBy_ShouldValidateSaleStatus(SaleStatus status, bool expectedResult)
    {
        // Arrange
        var sale = ActiveSaleSpecificationTestData.GenerateSale(status);
        var specification = new ActiveSaleSpecification();

        // Act
        var result = specification.IsSatisfiedBy(sale);

        // Assert
        result.Should().Be(expectedResult);
    }
}
