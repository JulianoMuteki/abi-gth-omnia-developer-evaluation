using Ambev.DeveloperEvaluation.Application.Sales.EditSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Enums;
using AutoMapper;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

/// <summary>
/// Contains unit tests for the <see cref="EditSaleHandler"/> class.
/// </summary>
public class EditSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IPublisher _publisher;
    private readonly EditSaleHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="EditSaleHandlerTests"/> class.
    /// </summary>
    public EditSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _publisher = Substitute.For<IPublisher>();
        _handler = new EditSaleHandler(_saleRepository, _mapper, _publisher);
    }

    /// <summary>
    /// Tests that a valid sale edit request returns success response.
    /// </summary>
    [Fact(DisplayName = "Given valid sale data When editing sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = EditSaleHandlerTestData.GenerateValidCommandWithConsistentIds();
        var itemIds = command.Items.Select(i => i.Id).Where(id => id.HasValue).Select(id => id!.Value).ToList();
        var existingSale = EditSaleHandlerTestData.GenerateSaleWithConsistentIds(itemIds);
        var updatedSale = EditSaleHandlerTestData.GenerateSaleWithConsistentIds(itemIds);
        var result = EditSaleHandlerTestData.GenerateResult();

        existingSale.SaleNumber = command.SaleNumber;
        updatedSale.SaleNumber = command.SaleNumber;

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingSale);
        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(updatedSale);
        _mapper.Map<EditSaleResult>(updatedSale).Returns(result);

        // When
        var editSaleResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        editSaleResult.Should().NotBeNull();
        editSaleResult.Id.Should().Be(result.Id);
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await _publisher.Received(1).Publish(Arg.Any<SaleModifiedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid sale edit request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid sale data When editing sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new EditSaleCommand(); // Empty command will fail validation

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
    }

    /// <summary>
    /// Tests that editing a non-existent sale throws an exception.
    /// </summary>
    [Fact(DisplayName = "Given non-existent sale ID When editing sale Then throws invalid operation exception")]
    public async Task Handle_NonExistentSale_ThrowsInvalidOperationException()
    {
        // Given
        var command = EditSaleHandlerTestData.GenerateValidCommand();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Sale with ID {command.Id} not found");
    }

    /// <summary>
    /// Tests that editing a sale with an existing sale number throws an exception.
    /// </summary>
    [Fact(DisplayName = "Given existing sale number When editing sale Then throws invalid operation exception")]
    public async Task Handle_ExistingSaleNumber_ThrowsInvalidOperationException()
    {
        // Given
        var command = EditSaleHandlerTestData.GenerateValidCommandWithConsistentIds();
        var itemIds = command.Items.Select(i => i.Id).Where(id => id.HasValue).Select(id => id!.Value).ToList();
        var existingSale = EditSaleHandlerTestData.GenerateSaleWithConsistentIds(itemIds);
        var saleWithSameNumber = EditSaleHandlerTestData.GenerateSaleWithConsistentIds(itemIds);

        existingSale.SaleNumber = "SALE-1001";
        command.SaleNumber = "SALE-1002";

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingSale);
        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns(saleWithSameNumber);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Sale with number {command.SaleNumber} already exists");
    }

    /// <summary>
    /// Tests that editing a sale item that doesn't exist throws an exception.
    /// </summary>
    [Fact(DisplayName = "Given non-existent item ID When editing sale Then throws invalid operation exception")]
    public async Task Handle_NonExistentItem_ThrowsInvalidOperationException()
    {
        // Given
        var command = EditSaleHandlerTestData.GenerateValidCommandWithConsistentIds();
        var itemIds = command.Items.Select(i => i.Id).Where(id => id.HasValue).Select(id => id!.Value).ToList();
        var existingSale = EditSaleHandlerTestData.GenerateSaleWithConsistentIds(itemIds);
        var nonExistentItemId = Guid.NewGuid();

        existingSale.SaleNumber = command.SaleNumber;
        command.Items.First().Id = nonExistentItemId;

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingSale);
        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Sale item with ID {nonExistentItemId} not found in sale {command.Id}");
    }

    /// <summary>
    /// Tests that the sale modified event is published.
    /// </summary>
    [Fact(DisplayName = "Given valid sale edit When handling Then publishes sale modified event")]
    public async Task Handle_ValidRequest_PublishesSaleModifiedEvent()
    {
        // Given
        var command = EditSaleHandlerTestData.GenerateValidCommandWithConsistentIds();
        var itemIds = command.Items.Select(i => i.Id).Where(id => id.HasValue).Select(id => id!.Value).ToList();
        var existingSale = EditSaleHandlerTestData.GenerateSaleWithConsistentIds(itemIds);
        var updatedSale = EditSaleHandlerTestData.GenerateSaleWithConsistentIds(itemIds);
        var result = EditSaleHandlerTestData.GenerateResult();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingSale);
        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(updatedSale);
        _mapper.Map<EditSaleResult>(updatedSale).Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _publisher.Received(1).Publish(
            Arg.Is<SaleModifiedEvent>(e => e.Sale.Id == updatedSale.Id),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the sale cancelled event is published when sale status changes to cancelled.
    /// </summary>
    [Fact(DisplayName = "Given sale status change to cancelled When handling Then publishes sale cancelled event")]
    public async Task Handle_SaleStatusChangeToCancelled_PublishesSaleCancelledEvent()
    {
        // Given
        var command = EditSaleHandlerTestData.GenerateValidCommandWithConsistentIds();
        var itemIds = command.Items.Select(i => i.Id).Where(id => id.HasValue).Select(id => id!.Value).ToList();
        var existingSale = EditSaleHandlerTestData.GenerateSaleWithConsistentIds(itemIds);
        var updatedSale = EditSaleHandlerTestData.GenerateSaleWithConsistentIds(itemIds);

        existingSale.Status = SaleStatus.Active;
        command.Status = SaleStatus.Cancelled;
        updatedSale.Status = SaleStatus.Cancelled;

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingSale);
        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(updatedSale);
        _mapper.Map<EditSaleResult>(updatedSale).Returns(EditSaleHandlerTestData.GenerateResult());

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _publisher.Received(1).Publish(Arg.Any<SaleModifiedEvent>(), Arg.Any<CancellationToken>());
        await _publisher.Received(1).Publish(Arg.Any<SaleCancelledEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that item cancelled events are published when items are cancelled.
    /// </summary>
    [Fact(DisplayName = "Given item status change to cancelled When handling Then publishes item cancelled events")]
    public async Task Handle_ItemStatusChangeToCancelled_PublishesItemCancelledEvents()
    {
        // Given
        var command = EditSaleHandlerTestData.GenerateValidCommandWithConsistentIds();
        var itemIds = command.Items.Select(i => i.Id).Where(id => id.HasValue).Select(id => id!.Value).ToList();
        var existingSale = EditSaleHandlerTestData.GenerateSaleWithConsistentIds(itemIds);
        var updatedSale = EditSaleHandlerTestData.GenerateSaleWithConsistentIds(itemIds);

        // Set up existing item with Active status
        var existingItem = existingSale.Items.First();
        existingItem.Status = SaleItemStatus.Active;

        // Set up command to change item status to Cancelled
        var commandItem = command.Items.First();
        commandItem.Id = existingItem.Id;
        commandItem.Status = SaleItemStatus.Cancelled;

        // Ensure the updated sale reflects the cancelled item
        var updatedItem = updatedSale.Items.First();
        updatedItem.Id = existingItem.Id;
        updatedItem.Status = SaleItemStatus.Cancelled;

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingSale);
        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(updatedSale);
        _mapper.Map<EditSaleResult>(updatedSale).Returns(EditSaleHandlerTestData.GenerateResult());

        // Configure the mapper to properly map the status change
        _mapper.Map(Arg.Any<EditSaleItemCommand>(), Arg.Any<SaleItem>())
            .Returns(callInfo =>
            {
                var command = callInfo.Arg<EditSaleItemCommand>();
                var item = callInfo.Arg<SaleItem>();
                item.Status = command.Status; // Ensure status is mapped
                return item;
            });

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _publisher.Received(1).Publish(Arg.Any<SaleModifiedEvent>(), Arg.Any<CancellationToken>());
        await _publisher.Received(1).Publish(Arg.Any<ItemCancelledEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that new items are added to the sale.
    /// </summary>
    [Fact(DisplayName = "Given new item When editing sale Then adds new item")]
    public async Task Handle_NewItem_AddsNewItemToSale()
    {
        // Given
        var command = EditSaleHandlerTestData.GenerateValidCommandWithConsistentIds();
        var itemIds = command.Items.Select(i => i.Id).Where(id => id.HasValue).Select(id => id!.Value).ToList();
        var existingSale = EditSaleHandlerTestData.GenerateSaleWithConsistentIds(itemIds);
        var updatedSale = EditSaleHandlerTestData.GenerateSaleWithConsistentIds(itemIds);

        // Add a new item (without ID) to the command
        var newItemCommand = EditSaleHandlerTestData.GenerateValidItemCommand();
        newItemCommand.Id = null;
        command.Items.Add(newItemCommand);

        // Ensure the existing sale has fewer items than the command
        var originalItemCount = existingSale.Items.Count;

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingSale);
        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(updatedSale);
        _mapper.Map<EditSaleResult>(updatedSale).Returns(EditSaleHandlerTestData.GenerateResult());
        _mapper.Map<SaleItem>(newItemCommand).Returns(EditSaleHandlerTestData.GenerateSaleItem());

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).UpdateAsync(
            Arg.Is<Sale>(s => s.Items.Count > originalItemCount),
            Arg.Any<CancellationToken>());
    }
}
