namespace Fixture.Shop.Contracts.Orders;

public sealed record OrderReceipt(
    int OrderId,
    string OrderNumber,
    decimal TotalAmount);
