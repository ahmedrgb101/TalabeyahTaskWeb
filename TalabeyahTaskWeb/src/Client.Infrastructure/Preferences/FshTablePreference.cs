using FSH.WebApi.Shared.Notifications;

namespace TalabeyahTaskWeb.Client.Infrastructure.Preferences;
public class FshTablePreference : INotificationMessage
{
    public bool IsDense { get; set; }
    public bool IsStriped { get; set; }
    public bool HasBorder { get; set; }
    public bool IsHoverable { get; set; }
}