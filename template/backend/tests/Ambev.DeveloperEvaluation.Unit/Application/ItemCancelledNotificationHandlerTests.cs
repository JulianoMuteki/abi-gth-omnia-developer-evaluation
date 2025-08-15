using Ambev.DeveloperEvaluation.Application.Sales.ItemCancelled;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application
{
    public class ItemCancelledNotificationHandlerTests
    {
        private readonly ILogger<ItemCancelledNotificationHandler> _loggerMock;
        private readonly ItemCancelledNotificationHandler _handler;

        public ItemCancelledNotificationHandlerTests()
        {
            _loggerMock = Substitute.For<ILogger<ItemCancelledNotificationHandler>>();
            _handler = new ItemCancelledNotificationHandler(_loggerMock);
        }

        [Fact]
        public async Task Handle_WhenItemCancelledEventReceived_ShouldLogInformation()
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

            var saleItem = new SaleItem
            {
                Id = Guid.NewGuid(),
                SaleId = sale.Id,
                ProductName = "Test Product",
                Quantity = 5,
                UnitPrice = 10.00m,
                TotalItemAmount = 50.00m,
                Status = SaleItemStatus.Cancelled,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var itemCancelledEvent = new ItemCancelledEvent(saleItem, sale);

            // Act
            await _handler.Handle(itemCancelledEvent, CancellationToken.None);

            // Assert
            _loggerMock.Received(1).Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<object>(v => v.ToString().Contains("ItemCancelled event published")),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception, string>>()
            );
        }

        [Fact]
        public async Task Handle_WhenItemCancelledEventReceived_ShouldLogCorrectItemDetails()
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

            var saleItem = new SaleItem
            {
                Id = Guid.NewGuid(),
                SaleId = sale.Id,
                ProductName = "Premium Product",
                Quantity = 3,
                UnitPrice = 25.00m,
                TotalItemAmount = 75.00m,
                Status = SaleItemStatus.Cancelled,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var itemCancelledEvent = new ItemCancelledEvent(saleItem, sale);

            // Act
            await _handler.Handle(itemCancelledEvent, CancellationToken.None);

            // Assert
            _loggerMock.Received(1).Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<object>(v => 
                    v.ToString().Contains("SALE-123") &&
                    v.ToString().Contains("Premium Product") &&
                    v.ToString().Contains("3") &&
                    v.ToString().Contains("25.00") &&
                    v.ToString().Contains("75.00")
                ),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception, string>>()
            );
        }
    }
}
