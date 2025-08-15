using Ambev.DeveloperEvaluation.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.SaleCreated
{
    public class SaleCreatedNotificationHandler : INotificationHandler<SaleCreatedEvent>
    {
        private readonly ILogger<SaleCreatedNotificationHandler> _logger;

        public SaleCreatedNotificationHandler(ILogger<SaleCreatedNotificationHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(SaleCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "SaleCreated event published - Sale Number: {SaleNumber}, Customer: {Customer}, Total Amount: {TotalAmount}, Branch: {Branch}",
                notification.Sale.SaleNumber,
                notification.Sale.CustomerName,
                notification.Sale.TotalAmount,
                notification.Sale.BranchName
            );

            // Here you could add actual message broker publishing logic
            // For example: await _messageBroker.PublishAsync("sale.created", notification.Sale);
            
            await Task.CompletedTask;
        }
    }
}
