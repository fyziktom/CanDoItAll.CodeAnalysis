namespace Fixture.Shop.Contracts.Orders;

/// <summary>
/// Coordinates order creation and confirmation work.
/// </summary>
public interface IOrderService
{
    Task<OrderReceipt> PlaceOrderAsync(PlaceOrderCommand command, CancellationToken cancellationToken = default);
}
