using Fixture.Shop.Application.Orders;
using Fixture.Shop.Infrastructure.Persistence.Entities;

namespace Fixture.Shop.Application.Notifications;

public sealed class OrderReceiptComposer
{
    private readonly OrderNumberFormatter _orderNumberFormatter;

    public OrderReceiptComposer(OrderNumberFormatter orderNumberFormatter)
    {
        _orderNumberFormatter = orderNumberFormatter;
    }

    public string Compose(Order order)
    {
        return $"Order {_orderNumberFormatter.Format(order.Id)} totals {order.TotalAmount:C}.";
    }
}
