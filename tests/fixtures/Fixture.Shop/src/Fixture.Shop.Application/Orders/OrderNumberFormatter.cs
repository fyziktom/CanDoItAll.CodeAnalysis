namespace Fixture.Shop.Application.Orders;

public sealed class OrderNumberFormatter
{
    public string Format(int orderId)
    {
        return $"ORD-{orderId:D6}";
    }
}
