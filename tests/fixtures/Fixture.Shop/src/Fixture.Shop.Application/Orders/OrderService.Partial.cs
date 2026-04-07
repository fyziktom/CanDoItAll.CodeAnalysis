using Fixture.Shop.Contracts.Orders;
using Fixture.Shop.Infrastructure.Persistence.Entities;

namespace Fixture.Shop.Application.Orders;

public sealed partial class OrderService
{
    private static Order CreateOrder(PlaceOrderCommand command, Customer customer)
    {
        var order = new Order
        {
            Customer = customer,
            Status = OrderStatus.Pending,
            Lines = command.Lines
                .Select(
                    line => new OrderLine
                    {
                        Sku = line.Sku,
                        Quantity = line.Quantity,
                        UnitPrice = line.UnitPrice,
                    })
                .ToList(),
        };

        order.TotalAmount = order.Lines.Sum(line => line.UnitPrice * line.Quantity);
        return order;
    }
}
