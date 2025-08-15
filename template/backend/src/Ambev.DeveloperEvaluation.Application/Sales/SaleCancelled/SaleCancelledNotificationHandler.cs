using Ambev.DeveloperEvaluation.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.SaleCancelled
{
    public class SaleCancelledNotificationHandler : INotificationHandler<SaleCancelledEvent>
    {
        private readonly ILogger<SaleCancelledNotificationHandler> _logger;

        public SaleCancelledNotificationHandler(ILogger<SaleCancelledNotificationHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(SaleCancelledEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "SaleCancelled event published - Sale Number: {SaleNumber}, Customer: {Customer}, Total Amount: {TotalAmount}, Branch: {Branch}, Cancelled At: {CancelledAt}, Cancelled By: {CancelledBy}",
                notification.Sale.SaleNumber,
                notification.Sale.CustomerName,
                notification.Sale.TotalAmount,
                notification.Sale.BranchName,
                notification.Sale.CancelledAt,
                notification.Sale.CancelledBy
            );

            // Here you could add actual message broker publishing logic
            // For example: await _messageBroker.PublishAsync("sale.cancelled", notification.Sale);
            
            await Task.CompletedTask;
        }
    }
}
