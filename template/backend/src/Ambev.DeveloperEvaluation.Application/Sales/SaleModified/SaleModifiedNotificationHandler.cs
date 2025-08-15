using Ambev.DeveloperEvaluation.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.SaleModified
{
    public class SaleModifiedNotificationHandler : INotificationHandler<SaleModifiedEvent>
    {
        private readonly ILogger<SaleModifiedNotificationHandler> _logger;

        public SaleModifiedNotificationHandler(ILogger<SaleModifiedNotificationHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(SaleModifiedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "SaleModified event published - Sale Number: {SaleNumber}, Customer: {Customer}, Total Amount: {TotalAmount}, Branch: {Branch}, Updated At: {UpdatedAt}",
                notification.Sale.SaleNumber,
                notification.Sale.CustomerName,
                notification.Sale.TotalAmount,
                notification.Sale.BranchName,
                notification.Sale.UpdatedAt
            );

            // Here you could add actual message broker publishing logic
            // For example: await _messageBroker.PublishAsync("sale.modified", notification.Sale);
            
            await Task.CompletedTask;
        }
    }
}
