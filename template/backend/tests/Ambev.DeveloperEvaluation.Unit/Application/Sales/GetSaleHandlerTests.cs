using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

/// <summary>
/// Contains unit tests for the <see cref="GetSaleHandler"/> class.
/// </summary>
public class GetSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSaleHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSaleHandlerTests"/> class.
    /// </summary>
    public GetSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSaleHandler(_saleRepository, _mapper);
    }

    /// <summary>
    /// Tests that a valid get sale request returns the sale details.
    /// </summary>
    [Fact(DisplayName = "Given valid request When getting sale Then returns sale details")]
    public async Task Handle_ValidRequest_ReturnsSaleDetails()
    {
        // Given
        var command = GetSaleHandlerTestData.GenerateValidCommand();
        var sale = GetSaleHandlerTestData.GenerateSale();
        var result = GetSaleHandlerTestData.GenerateResult();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<GetSaleResult>(sale).Returns(result);

        // When
        var getSaleResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        getSaleResult.Should().NotBeNull();
        getSaleResult.Id.Should().Be(result.Id);
        getSaleResult.SaleNumber.Should().Be(result.SaleNumber);
        await _saleRepository.Received(1).GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid get sale request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid request When getting sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new GetSaleCommand(); // Empty command will fail validation

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
    }

    /// <summary>
    /// Tests that requesting a non-existent sale throws a key not found exception.
    /// </summary>
    [Fact(DisplayName = "Given non-existent sale ID When getting sale Then throws key not found exception")]
    public async Task Handle_NonExistentSale_ThrowsKeyNotFoundException()
    {
        // Given
        var command = GetSaleHandlerTestData.GenerateValidCommand();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {command.Id} not found");
    }

    /// <summary>
    /// Tests that the repository is called with the correct sale ID.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then calls repository with correct ID")]
    public async Task Handle_ValidRequest_CallsRepositoryWithCorrectId()
    {
        // Given
        var command = GetSaleHandlerTestData.GenerateValidCommand();
        var sale = GetSaleHandlerTestData.GenerateSale();
        var result = GetSaleHandlerTestData.GenerateResult();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<GetSaleResult>(sale).Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the mapper is called with the correct sale entity.
    /// </summary>
    [Fact(DisplayName = "Given valid sale When handling Then maps sale to result")]
    public async Task Handle_ValidRequest_MapsSaleToResult()
    {
        // Given
        var command = GetSaleHandlerTestData.GenerateValidCommand();
        var sale = GetSaleHandlerTestData.GenerateSale();
        var result = GetSaleHandlerTestData.GenerateResult();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<GetSaleResult>(sale).Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1).Map<GetSaleResult>(Arg.Is<Sale>(s => s.Id == sale.Id));
    }

    /// <summary>
    /// Tests that the result contains the expected sale information.
    /// </summary>
    [Fact(DisplayName = "Given valid sale When mapping Then result contains correct information")]
    public async Task Handle_ValidRequest_ResultContainsCorrectInformation()
    {
        // Given
        var command = GetSaleHandlerTestData.GenerateValidCommand();
        var sale = GetSaleHandlerTestData.GenerateSale();
        var result = GetSaleHandlerTestData.GenerateResult();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<GetSaleResult>(sale).Returns(result);

        // When
        var getSaleResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        getSaleResult.Should().NotBeNull();
        getSaleResult.Id.Should().Be(result.Id);
        getSaleResult.SaleNumber.Should().Be(result.SaleNumber);
        getSaleResult.CustomerName.Should().Be(result.CustomerName);
        getSaleResult.BranchName.Should().Be(result.BranchName);
        getSaleResult.TotalAmount.Should().Be(result.TotalAmount);
        getSaleResult.Items.Should().HaveCount(result.Items.Count);
    }
}
