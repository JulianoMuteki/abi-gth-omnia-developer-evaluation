using Ambev.DeveloperEvaluation.Application.Sales.SaleCreated;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application
{
    public class SaleCreatedNotificationHandlerTests
    {
        private readonly ILogger<SaleCreatedNotificationHandler> _loggerMock;
        private readonly SaleCreatedNotificationHandler _handler;

        public SaleCreatedNotificationHandlerTests()
        {
            _loggerMock = Substitute.For<ILogger<SaleCreatedNotificationHandler>>();
            _handler = new SaleCreatedNotificationHandler(_loggerMock);
        }

        [Fact]
        public async Task Handle_WhenSaleCreatedEventReceived_ShouldLogInformation()
        {
            // Arrange
            var sale = new Sale
            {
                Id = Guid.NewGuid(),
                SaleNumber = "TEST-001",
                CustomerName = "John Doe",
                TotalAmount = 100.00m,
                BranchName = "Main Branch",
                Status = SaleStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            var saleCreatedEvent = new SaleCreatedEvent(sale);

            // Act
            await _handler.Handle(saleCreatedEvent, CancellationToken.None);

            // Assert
            _loggerMock.Received(1).Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<object>(v => v.ToString().Contains("SaleCreated event published")),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception, string>>()
            );
        }

        [Fact]
        public async Task Handle_WhenSaleCreatedEventReceived_ShouldLogCorrectSaleDetails()
        {
            // Arrange
            var sale = new Sale
            {
                Id = Guid.NewGuid(),
                SaleNumber = "SALE-123",
                CustomerName = "Jane Smith",
                TotalAmount = 250.50m,
                BranchName = "Downtown Branch",
                Status = SaleStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            var saleCreatedEvent = new SaleCreatedEvent(sale);

            // Act
            await _handler.Handle(saleCreatedEvent, CancellationToken.None);

            // Assert
            _loggerMock.Received(1).Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<object>(v => 
                    v.ToString().Contains("SALE-123") &&
                    v.ToString().Contains("Jane Smith") &&
                    v.ToString().Contains("250.50") &&
                    v.ToString().Contains("Downtown Branch")
                ),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception, string>>()
            );
        }
    }
}
