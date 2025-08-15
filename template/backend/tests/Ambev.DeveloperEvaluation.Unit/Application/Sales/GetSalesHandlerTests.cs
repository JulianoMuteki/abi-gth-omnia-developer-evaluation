using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Domain.Entities;
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
/// Contains unit tests for the <see cref="GetSalesHandler"/> class.
/// </summary>
public class GetSalesHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSalesHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSalesHandlerTests"/> class.
    /// </summary>
    public GetSalesHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSalesHandler(_saleRepository, _mapper);
    }

    /// <summary>
    /// Tests that a valid get sales request returns paginated results.
    /// </summary>
    [Fact(DisplayName = "Given valid request When getting sales Then returns paginated results")]
    public async Task Handle_ValidRequest_ReturnsPaginatedResults()
    {
        // Given
        var command = GetSalesHandlerTestData.GenerateValidCommand();
        command.PageSize = 5; // Explicitly set page size to 5
        var sales = GetSalesHandlerTestData.GenerateSales(5);
        var salesItems = GetSalesHandlerTestData.GenerateSalesItems(5);
        const int totalCount = 15;

        _saleRepository.GetPaginatedAsync(
            command.PageNumber,
            command.PageSize,
            command.CustomerId,
            command.BranchId,
            command.StartDate,
            command.EndDate,
            command.Status,
            Arg.Any<CancellationToken>())
            .Returns((sales, totalCount));

        _mapper.Map<List<GetSalesItemResult>>(sales).Returns(salesItems);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Sales.Should().HaveCount(5);
        result.CurrentPage.Should().Be(command.PageNumber);
        result.PageSize.Should().Be(command.PageSize);
        result.TotalCount.Should().Be(totalCount);
        result.TotalPages.Should().Be(3); // Math.Ceiling(15 / 5) = 3
    }

    /// <summary>
    /// Tests that an invalid get sales request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid request When getting sales Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new GetSalesCommand
        {
            PageNumber = 0, // Invalid: must be greater than 0
            PageSize = 0    // Invalid: must be greater than 0
        };

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
    }

    /// <summary>
    /// Tests that the repository is called with correct parameters.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then calls repository with correct parameters")]
    public async Task Handle_ValidRequest_CallsRepositoryWithCorrectParameters()
    {
        // Given
        var command = GetSalesHandlerTestData.GenerateValidCommand();
        var sales = GetSalesHandlerTestData.GenerateSales(3);
        var salesItems = GetSalesHandlerTestData.GenerateSalesItems(3);
        const int totalCount = 3;

        _saleRepository.GetPaginatedAsync(
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<Guid?>(),
            Arg.Any<Guid?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>())
            .Returns((sales, totalCount));

        _mapper.Map<List<GetSalesItemResult>>(sales).Returns(salesItems);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).GetPaginatedAsync(
            command.PageNumber,
            command.PageSize,
            command.CustomerId,
            command.BranchId,
            command.StartDate,
            command.EndDate,
            command.Status,
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the mapper is called with the correct sales list.
    /// </summary>
    [Fact(DisplayName = "Given valid sales When handling Then maps sales to result items")]
    public async Task Handle_ValidRequest_MapsSalesToResultItems()
    {
        // Given
        var command = GetSalesHandlerTestData.GenerateValidCommand();
        var sales = GetSalesHandlerTestData.GenerateSales(3);
        var salesItems = GetSalesHandlerTestData.GenerateSalesItems(3);
        const int totalCount = 3;

        _saleRepository.GetPaginatedAsync(
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<Guid?>(),
            Arg.Any<Guid?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>())
            .Returns((sales, totalCount));

        _mapper.Map<List<GetSalesItemResult>>(sales).Returns(salesItems);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1).Map<List<GetSalesItemResult>>(Arg.Is<List<Sale>>(s => s.Count == 3));
    }

    /// <summary>
    /// Tests that empty results are handled correctly.
    /// </summary>
    [Fact(DisplayName = "Given no sales When getting sales Then returns empty result")]
    public async Task Handle_NoSales_ReturnsEmptyResult()
    {
        // Given
        var command = GetSalesHandlerTestData.GenerateValidCommand();
        var emptySales = new List<Sale>();
        var emptySalesItems = new List<GetSalesItemResult>();
        const int totalCount = 0;

        _saleRepository.GetPaginatedAsync(
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<Guid?>(),
            Arg.Any<Guid?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>())
            .Returns((emptySales, totalCount));

        _mapper.Map<List<GetSalesItemResult>>(emptySales).Returns(emptySalesItems);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Sales.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    /// <summary>
    /// Tests that pagination calculation is correct for last page.
    /// </summary>
    [Fact(DisplayName = "Given sales with remainder When calculating pages Then rounds up correctly")]
    public async Task Handle_SalesWithRemainder_CalculatesPagesCorrectly()
    {
        // Given
        var command = GetSalesHandlerTestData.GenerateValidCommand();
        command.PageSize = 3;
        var sales = GetSalesHandlerTestData.GenerateSales(2);
        var salesItems = GetSalesHandlerTestData.GenerateSalesItems(2);
        const int totalCount = 8; // 8 total items, 3 per page = 3 pages

        _saleRepository.GetPaginatedAsync(
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<Guid?>(),
            Arg.Any<Guid?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<DateTime?>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>())
            .Returns((sales, totalCount));

        _mapper.Map<List<GetSalesItemResult>>(sales).Returns(salesItems);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.TotalPages.Should().Be(3); // Math.Ceiling(8 / 3) = 3
    }
}
