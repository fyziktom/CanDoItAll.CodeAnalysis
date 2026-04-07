namespace Fixture.Shop.Infrastructure.Persistence.Entities;

public sealed class Order
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public Customer Customer { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public OrderStatus Status { get; set; }

    public List<OrderLine> Lines { get; set; } = [];
}
