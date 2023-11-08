using FSH.WebApi.Shared.Notifications;

namespace TalabeyahTaskWeb.Client.Infrastructure.Notifications;
public record ConnectionStateChanged(ConnectionState State, string? Message) : INotificationMessage;