namespace Fixture.Shop.Infrastructure.Persistence.Entities;

/// <summary>
/// Represents a customer that can place orders.
/// </summary>
public sealed class Customer
{
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public CustomerPreferences Preferences { get; set; } = new();

    public List<Order> Orders { get; set; } = [];
}
