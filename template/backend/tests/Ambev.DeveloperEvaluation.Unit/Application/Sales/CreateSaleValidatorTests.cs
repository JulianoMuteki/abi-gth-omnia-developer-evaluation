using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using FluentAssertions;
using FluentValidation;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

/// <summary>
/// Contains unit tests for the <see cref="CreateSaleCommandValidator"/> class.
/// </summary>
public class CreateSaleValidatorTests
{
    private readonly CreateSaleCommandValidator _validator;
    private readonly CreateSaleItemCommandValidator _itemValidator;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSaleValidatorTests"/> class.
    /// </summary>
    public CreateSaleValidatorTests()
    {
        _validator = new CreateSaleCommandValidator();
        _itemValidator = new CreateSaleItemCommandValidator();
    }

    #region CreateSaleCommandValidator Tests

    /// <summary>
    /// Tests that a valid CreateSaleCommand passes validation.
    /// </summary>
    [Fact(DisplayName = "Given valid command When validated Then passes validation")]
    public void Validate_ValidCommand_PassesValidation()
    {
        // Given
        var command = CreateSaleValidatorTestData.GenerateValidCommand();

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that an empty sale number fails validation.
    /// </summary>
    [Fact(DisplayName = "Given empty sale number When validated Then fails validation")]
    public void Validate_EmptySaleNumber_FailsValidation()
    {
        // Given
        var command = CreateSaleValidatorTestData.GenerateValidCommand();
        command.SaleNumber = string.Empty;

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(command.SaleNumber) && e.ErrorMessage == "Sale number is required");
    }

    /// <summary>
    /// Tests that a sale number exceeding maximum length fails validation.
    /// </summary>
    [Fact(DisplayName = "Given sale number exceeding max length When validated Then fails validation")]
    public void Validate_SaleNumberExceedingMaxLength_FailsValidation()
    {
        // Given
        var command = CreateSaleValidatorTestData.GenerateValidCommand();
        command.SaleNumber = new string('A', 51);

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(command.SaleNumber) && e.ErrorMessage == "Sale number cannot exceed 50 characters");
    }

    /// <summary>
    /// Tests that a future sale date fails validation.
    /// </summary>
    [Fact(DisplayName = "Given future sale date When validated Then fails validation")]
    public void Validate_FutureSaleDate_FailsValidation()
    {
        // Given
        var command = CreateSaleValidatorTestData.GenerateValidCommand();
        command.SaleDate = DateTime.UtcNow.AddDays(1);

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(command.SaleDate) && e.ErrorMessage == "Sale date cannot be in the future");
    }

    /// <summary>
    /// Tests that an empty customer name fails validation.
    /// </summary>
    [Fact(DisplayName = "Given empty customer name When validated Then fails validation")]
    public void Validate_EmptyCustomerName_FailsValidation()
    {
        // Given
        var command = CreateSaleValidatorTestData.GenerateValidCommand();
        command.CustomerName = string.Empty;

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(command.CustomerName) && e.ErrorMessage == "Customer name is required");
    }

    /// <summary>
    /// Tests that an invalid customer email fails validation.
    /// </summary>
    [Fact(DisplayName = "Given invalid customer email When validated Then fails validation")]
    public void Validate_InvalidCustomerEmail_FailsValidation()
    {
        // Given
        var command = CreateSaleValidatorTestData.GenerateValidCommand();
        command.CustomerEmail = "invalid-email";

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(command.CustomerEmail) && e.ErrorMessage == "Customer email must be a valid email address");
    }

    /// <summary>
    /// Tests that an empty items list fails validation.
    /// </summary>
    [Fact(DisplayName = "Given empty items list When validated Then fails validation")]
    public void Validate_EmptyItemsList_FailsValidation()
    {
        // Given
        var command = CreateSaleValidatorTestData.GenerateValidCommand();
        command.Items.Clear();

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(command.Items) && e.ErrorMessage == "At least one sale item is required");
    }

    #endregion

    #region CreateSaleItemCommandValidator Tests

    /// <summary>
    /// Tests that a valid CreateSaleItemCommand passes validation.
    /// </summary>
    [Fact(DisplayName = "Given valid item command When validated Then passes validation")]
    public void Validate_ValidItemCommand_PassesValidation()
    {
        // Given
        var itemCommand = CreateSaleValidatorTestData.GenerateValidItemCommand();

        // When
        var result = _itemValidator.Validate(itemCommand);

        // Then
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that a zero quantity fails validation.
    /// </summary>
    [Fact(DisplayName = "Given zero quantity When validated Then fails validation")]
    public void Validate_ZeroQuantity_FailsValidation()
    {
        // Given
        var itemCommand = CreateSaleValidatorTestData.GenerateValidItemCommand();
        itemCommand.Quantity = 0;

        // When
        var result = _itemValidator.Validate(itemCommand);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(itemCommand.Quantity) && e.ErrorMessage == "Quantity must be greater than zero");
    }

    /// <summary>
    /// Tests that a quantity exceeding maximum fails validation.
    /// </summary>
    [Fact(DisplayName = "Given quantity exceeding max When validated Then fails validation")]
    public void Validate_QuantityExceedingMax_FailsValidation()
    {
        // Given
        var itemCommand = CreateSaleValidatorTestData.GenerateValidItemCommand();
        itemCommand.Quantity = 21;

        // When
        var result = _itemValidator.Validate(itemCommand);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(itemCommand.Quantity) && e.ErrorMessage == "Quantity cannot exceed 20");
    }

    /// <summary>
    /// Tests that a zero unit price fails validation.
    /// </summary>
    [Fact(DisplayName = "Given zero unit price When validated Then fails validation")]
    public void Validate_ZeroUnitPrice_FailsValidation()
    {
        // Given
        var itemCommand = CreateSaleValidatorTestData.GenerateValidItemCommand();
        itemCommand.UnitPrice = 0;

        // When
        var result = _itemValidator.Validate(itemCommand);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(itemCommand.UnitPrice) && e.ErrorMessage == "Unit price must be greater than zero");
    }

    /// <summary>
    /// Tests that a negative discount percentage fails validation.
    /// </summary>
    [Fact(DisplayName = "Given negative discount percentage When validated Then fails validation")]
    public void Validate_NegativeDiscountPercentage_FailsValidation()
    {
        // Given
        var itemCommand = CreateSaleValidatorTestData.GenerateValidItemCommand();
        itemCommand.DiscountPercentage = -1;

        // When
        var result = _itemValidator.Validate(itemCommand);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(itemCommand.DiscountPercentage) && e.ErrorMessage == "Discount percentage cannot be negative");
    }

    /// <summary>
    /// Tests that a discount percentage exceeding 100% fails validation.
    /// </summary>
    [Fact(DisplayName = "Given discount percentage exceeding 100% When validated Then fails validation")]
    public void Validate_DiscountPercentageExceeding100_FailsValidation()
    {
        // Given
        var itemCommand = CreateSaleValidatorTestData.GenerateValidItemCommand();
        itemCommand.DiscountPercentage = 101;

        // When
        var result = _itemValidator.Validate(itemCommand);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(itemCommand.DiscountPercentage) && e.ErrorMessage == "Discount percentage cannot exceed 100%");
    }

    /// <summary>
    /// Tests that an empty product name fails validation.
    /// </summary>
    [Fact(DisplayName = "Given empty product name When validated Then fails validation")]
    public void Validate_EmptyProductName_FailsValidation()
    {
        // Given
        var itemCommand = CreateSaleValidatorTestData.GenerateValidItemCommand();
        itemCommand.ProductName = string.Empty;

        // When
        var result = _itemValidator.Validate(itemCommand);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(itemCommand.ProductName) && e.ErrorMessage == "Product name is required");
    }

    #endregion
}
