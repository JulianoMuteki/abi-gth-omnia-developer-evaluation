using Ambev.DeveloperEvaluation.Application.Sales.SaleModified;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application
{
    public class SaleModifiedNotificationHandlerTests
    {
        private readonly ILogger<SaleModifiedNotificationHandler> _loggerMock;
        private readonly SaleModifiedNotificationHandler _handler;

        public SaleModifiedNotificationHandlerTests()
        {
            _loggerMock = Substitute.For<ILogger<SaleModifiedNotificationHandler>>();
            _handler = new SaleModifiedNotificationHandler(_loggerMock);
        }

        [Fact]
        public async Task Handle_WhenSaleModifiedEventReceived_ShouldLogInformation()
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
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var saleModifiedEvent = new SaleModifiedEvent(sale);

            // Act
            await _handler.Handle(saleModifiedEvent, CancellationToken.None);

            // Assert
            _loggerMock.Received(1).Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<object>(v => v.ToString().Contains("SaleModified event published")),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception, string>>()
            );
        }

        [Fact]
        public async Task Handle_WhenSaleModifiedEventReceived_ShouldLogCorrectSaleDetails()
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
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow.AddHours(1)
            };

            var saleModifiedEvent = new SaleModifiedEvent(sale);

            // Act
            await _handler.Handle(saleModifiedEvent, CancellationToken.None);

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
