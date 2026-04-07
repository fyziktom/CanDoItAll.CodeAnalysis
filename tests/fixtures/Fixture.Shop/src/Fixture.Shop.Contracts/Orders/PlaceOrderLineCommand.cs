namespace Fixture.Shop.Contracts.Orders;

public sealed record PlaceOrderLineCommand(
    string Sku,
    int Quantity,
    decimal UnitPrice);
