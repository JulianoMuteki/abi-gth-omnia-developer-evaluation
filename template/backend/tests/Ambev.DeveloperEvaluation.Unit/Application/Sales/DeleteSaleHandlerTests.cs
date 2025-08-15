using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

/// <summary>
/// Contains unit tests for the <see cref="DeleteSaleHandler"/> class.
/// </summary>
public class DeleteSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly DeleteSaleHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteSaleHandlerTests"/> class.
    /// </summary>
    public DeleteSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _handler = new DeleteSaleHandler(_saleRepository);
    }

    /// <summary>
    /// Tests that a valid sale deletion request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid sale ID When deleting sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = DeleteSaleHandlerTestData.GenerateValidCommand();
        var sale = DeleteSaleHandlerTestData.GenerateSale();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        // When
        var deleteSaleResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        deleteSaleResult.Should().NotBeNull();
        deleteSaleResult.Success.Should().BeTrue();
        await _saleRepository.Received(1).GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
        await _saleRepository.Received(1).DeleteAsync(sale, Arg.Any<CancellationToken>());
        await _saleRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that deleting a non-existent sale throws an exception.
    /// </summary>
    [Fact(DisplayName = "Given non-existent sale ID When deleting sale Then throws invalid operation exception")]
    public async Task Handle_NonExistentSale_ThrowsInvalidOperationException()
    {
        // Given
        var command = DeleteSaleHandlerTestData.GenerateValidCommand();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Sale with ID {command.Id} not found.");
    }

    /// <summary>
    /// Tests that the repository is called with the correct sale ID.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then calls repository with correct ID")]
    public async Task Handle_ValidRequest_CallsRepositoryWithCorrectId()
    {
        // Given
        var command = DeleteSaleHandlerTestData.GenerateValidCommand();
        var sale = DeleteSaleHandlerTestData.GenerateSale();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the sale is deleted from the repository.
    /// </summary>
    [Fact(DisplayName = "Given valid sale When handling Then deletes sale from repository")]
    public async Task Handle_ValidRequest_DeletesSaleFromRepository()
    {
        // Given
        var command = DeleteSaleHandlerTestData.GenerateValidCommand();
        var sale = DeleteSaleHandlerTestData.GenerateSale();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).DeleteAsync(
            Arg.Is<Sale>(s => s.Id == sale.Id),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that changes are saved to the repository.
    /// </summary>
    [Fact(DisplayName = "Given valid sale deletion When handling Then saves changes to repository")]
    public async Task Handle_ValidRequest_SavesChangesToRepository()
    {
        // Given
        var command = DeleteSaleHandlerTestData.GenerateValidCommand();
        var sale = DeleteSaleHandlerTestData.GenerateSale();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the result contains the expected success information.
    /// </summary>
    [Fact(DisplayName = "Given successful deletion When handling Then result contains success information")]
    public async Task Handle_ValidRequest_ResultContainsSuccessInformation()
    {
        // Given
        var command = DeleteSaleHandlerTestData.GenerateValidCommand();
        var sale = DeleteSaleHandlerTestData.GenerateSale();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        // When
        var deleteSaleResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        deleteSaleResult.Should().NotBeNull();
        deleteSaleResult.Success.Should().BeTrue();
    }
}
