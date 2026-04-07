using Fixture.Shop.Contracts.Notifications;

namespace Fixture.Shop.Infrastructure.Notifications;

public sealed class EmailNotificationSender : INotificationSender
{
    private readonly string _smtpEndpoint;

    public EmailNotificationSender(string smtpEndpoint)
    {
        _smtpEndpoint = smtpEndpoint;
    }

    public Task SendAsync(string emailAddress, string message, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
