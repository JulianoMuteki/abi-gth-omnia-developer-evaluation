using Ambev.DeveloperEvaluation.Application.Sales.SaleCancelled;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application
{
    public class SaleCancelledNotificationHandlerTests
    {
        private readonly ILogger<SaleCancelledNotificationHandler> _loggerMock;
        private readonly SaleCancelledNotificationHandler _handler;

        public SaleCancelledNotificationHandlerTests()
        {
            _loggerMock = Substitute.For<ILogger<SaleCancelledNotificationHandler>>();
            _handler = new SaleCancelledNotificationHandler(_loggerMock);
        }

        [Fact]
        public async Task Handle_WhenSaleCancelledEventReceived_ShouldLogInformation()
        {
            // Arrange
            var sale = new Sale
            {
                Id = Guid.NewGuid(),
                SaleNumber = "TEST-001",
                CustomerName = "John Doe",
                TotalAmount = 100.00m,
                BranchName = "Main Branch",
                Status = SaleStatus.Cancelled,
                CreatedAt = DateTime.UtcNow,
                CancelledAt = DateTime.UtcNow,
                CancelledBy = Guid.NewGuid()
            };

            var saleCancelledEvent = new SaleCancelledEvent(sale);

            // Act
            await _handler.Handle(saleCancelledEvent, CancellationToken.None);

            // Assert
            _loggerMock.Received(1).Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<object>(v => v.ToString().Contains("SaleCancelled event published")),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception, string>>()
            );
        }

        [Fact]
        public async Task Handle_WhenSaleCancelledEventReceived_ShouldLogCorrectSaleDetails()
        {
            // Arrange
            var cancelledBy = Guid.NewGuid();
            var cancelledAt = DateTime.UtcNow;
            var sale = new Sale
            {
                Id = Guid.NewGuid(),
                SaleNumber = "SALE-123",
                CustomerName = "Jane Smith",
                TotalAmount = 250.50m,
                BranchName = "Downtown Branch",
                Status = SaleStatus.Cancelled,
                CreatedAt = DateTime.UtcNow,
                CancelledAt = cancelledAt,
                CancelledBy = cancelledBy
            };

            var saleCancelledEvent = new SaleCancelledEvent(sale);

            // Act
            await _handler.Handle(saleCancelledEvent, CancellationToken.None);

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
