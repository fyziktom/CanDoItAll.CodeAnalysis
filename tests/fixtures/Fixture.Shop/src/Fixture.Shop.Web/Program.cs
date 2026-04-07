using Fixture.Shop.Application.Notifications;
using Fixture.Shop.Application.Orders;
using Fixture.Shop.Contracts.Notifications;
using Fixture.Shop.Contracts.Orders;
using Fixture.Shop.Contracts.Persistence;
using Fixture.Shop.Infrastructure.Notifications;
using Fixture.Shop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ShopDbContext>(
    options => options.UseSqlite("Data Source=fixture-shop.db"));
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<OrderNumberFormatter>();
builder.Services.AddScoped<OrderReceiptComposer>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
builder.Services.AddSingleton<TimeProvider>(_ => TimeProvider.System);
builder.Services.AddTransient<INotificationSender>(_ => new EmailNotificationSender("smtp://localhost"));

var app = builder.Build();

app.MapGet("/", () => "Fixture Shop");

app.Run();
