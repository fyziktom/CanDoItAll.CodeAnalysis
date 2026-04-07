namespace Fixture.Shop.Contracts.Notifications;

/// <summary>
/// Sends customer notifications after an order is accepted.
/// </summary>
public interface INotificationSender
{
    Task SendAsync(string emailAddress, string message, CancellationToken cancellationToken = default);
}
