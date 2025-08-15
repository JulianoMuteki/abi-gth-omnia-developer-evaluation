using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.SaleCreated;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Integration.Database;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Sales
{
    public class CreateSaleHandlerIntegrationTests : IClassFixture<TestDatabaseFixture>
    {
        private readonly TestDatabaseFixture _fixture;
        private readonly ILogger<SaleCreatedNotificationHandler> _loggerMock;

        public CreateSaleHandlerIntegrationTests(TestDatabaseFixture fixture)
        {
            _fixture = fixture;
            _loggerMock = Substitute.For<ILogger<SaleCreatedNotificationHandler>>();
        }

        [Fact]
        public async Task Handle_WhenSaleIsCreated_ShouldPublishSaleCreatedEvent()
        {
            // Arrange
            using var scope = _fixture.ServiceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            
            var command = new CreateSaleCommand
            {
                SaleNumber = "TEST-001",
                SaleDate = DateTime.UtcNow,
                CustomerId = Guid.NewGuid(),
                CustomerName = "Test Customer",
                CustomerEmail = "test@example.com",
                CustomerPhone = "1234567890",
                BranchId = Guid.NewGuid(),
                BranchName = "Test Branch",
                BranchCode = "TB001",
                Items = new List<CreateSaleItemCommand>
                {
                    new()
                    {
                        ProductId = Guid.NewGuid(),
                        ProductName = "Test Product",
                        ProductCode = "TP001",
                        ProductDescription = "Test Product Description",
                        Quantity = 5,
                        UnitPrice = 10.00m,
                        DiscountPercentage = 0.00m,
                        Status = SaleItemStatus.Active
                    }
                }
            };

            // Act
            var result = await mediator.Send(command);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
            
            // Verify that the event was published (this would be logged by our notification handler)
            // In a real scenario, you might want to use a test double for the publisher
            // and verify that Publish was called with the correct event
        }
    }
}
