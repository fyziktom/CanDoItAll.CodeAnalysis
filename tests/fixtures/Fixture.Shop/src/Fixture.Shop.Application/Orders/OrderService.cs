using Fixture.Shop.Application.Notifications;
using Fixture.Shop.Contracts.Notifications;
using Fixture.Shop.Contracts.Orders;
using Fixture.Shop.Infrastructure.Persistence;
using Fixture.Shop.Infrastructure.Persistence.Entities;

namespace Fixture.Shop.Application.Orders;

/// <summary>
/// Places orders against the primary shop database.
/// </summary>
public sealed partial class OrderService : IOrderService
{
    private readonly ShopDbContext _dbContext;
    private readonly INotificationSender _notificationSender;
    private readonly OrderReceiptComposer _receiptComposer;
    private readonly OrderNumberFormatter _orderNumberFormatter;

    public OrderService(
        ShopDbContext dbContext,
        INotificationSender notificationSender,
        OrderReceiptComposer receiptComposer,
        OrderNumberFormatter orderNumberFormatter)
    {
        _dbContext = dbContext;
        _notificationSender = notificationSender;
        _receiptComposer = receiptComposer;
        _orderNumberFormatter = orderNumberFormatter;
    }

    public async Task<OrderReceipt> PlaceOrderAsync(PlaceOrderCommand command, CancellationToken cancellationToken = default)
    {
        var customer = new Customer
        {
            Email = command.CustomerEmail,
        };

        var order = CreateOrder(command, customer);

        _dbContext.Customers.Add(customer);
        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _notificationSender.SendAsync(
            customer.Email,
            _receiptComposer.Compose(order),
            cancellationToken);

        return new OrderReceipt(order.Id, _orderNumberFormatter.Format(order.Id), order.TotalAmount);
    }
}
