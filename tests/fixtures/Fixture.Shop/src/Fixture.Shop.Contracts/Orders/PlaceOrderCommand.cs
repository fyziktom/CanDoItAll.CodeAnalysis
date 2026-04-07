namespace Fixture.Shop.Contracts.Orders;

public sealed record PlaceOrderCommand(
    string CustomerEmail,
    IReadOnlyList<PlaceOrderLineCommand> Lines);
