using Ambev.DeveloperEvaluation.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.ItemCancelled
{
    public class ItemCancelledNotificationHandler : INotificationHandler<ItemCancelledEvent>
    {
        private readonly ILogger<ItemCancelledNotificationHandler> _logger;

        public ItemCancelledNotificationHandler(ILogger<ItemCancelledNotificationHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(ItemCancelledEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "ItemCancelled event published - Sale Number: {SaleNumber}, Product: {ProductName}, Quantity: {Quantity}, Unit Price: {UnitPrice}, Total Amount: {TotalAmount}, Cancelled At: {CancelledAt}",
                notification.Sale.SaleNumber,
                notification.SaleItem.ProductName,
                notification.SaleItem.Quantity,
                notification.SaleItem.UnitPrice,
                notification.SaleItem.TotalItemAmount,
                notification.SaleItem.UpdatedAt
            );

            // Here you could add actual message broker publishing logic
            // For example: await _messageBroker.PublishAsync("item.cancelled", notification.SaleItem);
            
            await Task.CompletedTask;
        }
    }
}
