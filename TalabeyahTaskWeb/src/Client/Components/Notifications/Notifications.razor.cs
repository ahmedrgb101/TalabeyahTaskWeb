using FSH.WebApi.Shared.Notifications;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using TalabeyahTaskWeb.Client.Components.EntityTable;
using TalabeyahTaskWeb.Client.Infrastructure.ApiClient;
using TalabeyahTaskWeb.Client.Infrastructure.Auth;
using TalabeyahTaskWeb.Client.Infrastructure.Common;
using TalabeyahTaskWeb.Client.Infrastructure.Notifications;
using MudBlazor;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace TalabeyahTaskWeb.Client.Components.Notifications;
public partial class Notifications : IDisposable, IAsyncDisposable
{
    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;
    [Inject]
    private IAccessTokenProvider TokenProvider { get; set; } = default!;
    [Inject]
    private INotificationPublisher Publisher { get; set; } = default!;
    [Inject]
    private IAuthenticationService AuthService { get; set; } = default!;
    [Inject]
    private ILogger<NotificationConnection> Logger { get; set; } = default!;
    [Inject]

    private INotificationsClient NotificationsClient { get; set; } = default!;

    private readonly CancellationTokenSource _cts = new();
    private IDisposable? _subscription;
    private HubConnection? _hubConnection;
    private bool AlarmOn { get; set; }
    private string Icon { get; set; } = Icons.Material.Filled.NotificationsNone;
    private Color IconColor { get; set; } = Color.Inherit;
    private ICollection<NotificationDto> _notifications { get; set; }

    public ConnectionState ConnectionState =>
        _hubConnection?.State switch
        {
            HubConnectionState.Connected => ConnectionState.Connected,
            HubConnectionState.Disconnected => ConnectionState.Disconnected,
            _ => ConnectionState.Connecting
        };

    public string? ConnectionId => _hubConnection?.ConnectionId;

    protected override async Task<Task> OnInitializedAsync()
    {
        _notifications = (await NotificationsClient.GetNotificationsAsync()).Data;
        IconColor = _notifications.Any(x => !x.IsRead) ? Color.Error : Color.Inherit;
        Icon = _notifications.Any(x => !x.IsRead) ? Icons.Material.Filled.NotificationsActive : Icons.Material.Filled.NotificationsNone;

        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{Config[ConfigNames.ApiBaseUrl]}notifications", options =>
                options.AccessTokenProvider =
                    () => TokenProvider.GetAccessTokenAsync())
            .WithAutomaticReconnect(new IndefiniteRetryPolicy())
            .Build();

        _hubConnection.Reconnecting += ex =>
            OnConnectionStateChangedAsync(ConnectionState.Connecting, ex?.Message);

        _hubConnection.Reconnected += id =>
            OnConnectionStateChangedAsync(ConnectionState.Connected, id);

        _hubConnection.Closed += async ex =>
        {
            await OnConnectionStateChangedAsync(ConnectionState.Disconnected, ex?.Message);

            // This shouldn't happen with the IndefiniteRetryPolicy configured above,
            // but just in case it does, we wait a bit and restart the connection again.
            await Task.Delay(5000, _cts.Token);
            await ConnectWithRetryAsync(_cts.Token);
        };

        _subscription = _hubConnection.On<string, JsonObject>(NotificationConstants.NotificationFromServer, async (notificationTypeName, notificationJson) =>
        {
            _notifications = (await NotificationsClient.GetNotificationsAsync()).Data;
            Icon = _notifications.Any(x => !x.IsRead) ? Icons.Material.Filled.NotificationsActive : Icons.Material.Filled.NotificationsNone;
            IconColor = _notifications.Any(x => !x.IsRead) ? Color.Error : Color.Inherit;
            StateHasChanged();
        });

        // launch the signalR connection in the background.
        // see https://www.dotnetcurry.com/aspnet-core/realtime-app-using-blazor-webassembly-signalr-csharp9
        _ = ConnectWithRetryAsync(_cts.Token);

        return base.OnInitializedAsync();
    }

    protected virtual Task OnConnectionStateChangedAsync(ConnectionState state, string? message)
    {
        return Publisher.PublishAsync(new ConnectionStateChanged(state, message));
    }

    private async Task MarkAllAsReadAsync()
    {
        _ = await NotificationsClient.MarkAllAsReadAsync();
        Icon = Icons.Material.Filled.NotificationsNone;
        IconColor = Color.Inherit;
        _notifications = (await NotificationsClient.GetNotificationsAsync()).Data;
        StateHasChanged();
    }

    private async Task MarkAsReadAsync(Guid id)
    {
        _ = await NotificationsClient.MarkAsReadAsync(id);
        _notifications = (await NotificationsClient.GetNotificationsAsync()).Data;
        IconColor = _notifications.Any(x => !x.IsRead) ? Color.Error : Color.Inherit;
        Icon = _notifications.Any(x => !x.IsRead) ? Icons.Material.Filled.NotificationsActive : Icons.Material.Filled.NotificationsNone;
        StateHasChanged();
    }

    private async Task ConnectWithRetryAsync(CancellationToken cancellationToken)
    {
        _ = _hubConnection ?? throw new InvalidOperationException("HubConnection can't be null.");

        // Keep trying to until we can start or the token is canceled.
        while (true)
        {
            try
            {
                await _hubConnection.StartAsync(cancellationToken);
                await OnConnectionStateChangedAsync(ConnectionState.Connected, _hubConnection.ConnectionId);
                return;
            }
            catch when (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            catch (HttpRequestException requestException) when (requestException.StatusCode == HttpStatusCode.Unauthorized)
            {
                // This shouldn't happen, but just in case, redirect to logout.
                await AuthService.LogoutAsync();
                return;
            }
            catch
            {
                // Try again in a few seconds. This could be an incremental interval
                await Task.Delay(5000, cancellationToken);
            }
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
        _subscription?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}
