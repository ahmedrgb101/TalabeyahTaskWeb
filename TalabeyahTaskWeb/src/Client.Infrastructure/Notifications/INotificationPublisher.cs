using FSH.WebApi.Shared.Notifications;

namespace TalabeyahTaskWeb.Client.Infrastructure.Notifications;
public interface INotificationPublisher
{
    Task PublishAsync(INotificationMessage notification);
}