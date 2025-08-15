using Ambev.DeveloperEvaluation.Domain.Entities;
using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events
{
    public class ItemCancelledEvent : INotification
    {
        public SaleItem SaleItem { get; }
        public Sale Sale { get; }

        public ItemCancelledEvent(SaleItem saleItem, Sale sale)
        {
            SaleItem = saleItem;
            Sale = sale;
        }
    }
}
