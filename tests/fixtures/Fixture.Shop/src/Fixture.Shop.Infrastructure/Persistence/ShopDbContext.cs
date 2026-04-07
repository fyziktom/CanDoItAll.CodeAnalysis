using Fixture.Shop.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fixture.Shop.Infrastructure.Persistence;

/// <summary>
/// Stores customers, orders, and order lines for the fixture shop.
/// </summary>
public sealed class ShopDbContext : DbContext
{
    public ShopDbContext(DbContextOptions<ShopDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderLine> OrderLines => Set<OrderLine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>().ToTable("Customers", "sales");
        modelBuilder.Entity<Order>().ToTable("Orders", "sales");
        modelBuilder.Entity<OrderLine>().ToTable("OrderLines", "sales");

        modelBuilder.Entity<Order>()
            .Property(order => order.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Order>()
            .HasOne(order => order.Customer)
            .WithMany(customer => customer.Orders)
            .HasForeignKey(order => order.CustomerId);

        modelBuilder.Entity<Order>()
            .HasMany(order => order.Lines)
            .WithOne(line => line.Order)
            .HasForeignKey(line => line.OrderId);

        modelBuilder.Entity<Customer>().OwnsOne(
            customer => customer.Preferences,
            preferences =>
            {
                preferences.Property(value => value.MarketingOptIn).HasColumnName("MarketingOptIn");
            });
    }
}
