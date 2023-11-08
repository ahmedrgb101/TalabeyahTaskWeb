using FSH.WebApi.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using TalabeyahTaskWeb.Client.Infrastructure.Auth;
using TalabeyahTaskWeb.Client.Infrastructure.Common;
using System.Security.Claims;

namespace TalabeyahTaskWeb.Client.Shared;
public partial class NavMenu
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;

    [Inject]
    protected IAuthenticationService AuthServiceLog { get; set; } = default!;

    private string? _hangfireUrl;
    private bool _canViewHangfire;
    private bool _canViewDashboard;
    private bool _canViewTickets;
    private bool _canApplyTickets;
    private bool _canViewRoles;
    private bool _canViewUsers;
    private bool _canViewTenants;

    private bool _canViewCreatePORequest;
    private bool _canViewPORequestSteps;
    private bool _canViewPendingRequest;
    private bool _canViewVendor;
    private bool _canViewStore;
    private bool _canViewCategories;
    private bool _canViewitems;

    private bool CanViewAdministrationGroup => _canViewUsers || _canViewRoles || _canViewTenants;

    protected override async Task OnParametersSetAsync()
    {
        _hangfireUrl = Config[ConfigNames.ApiBaseUrl] + "jobs";
        var user = (await AuthState).User;
        _canViewHangfire = await AuthService.HasPermissionAsync(user, FSHAction.View, FSHResource.Hangfire);
        _canViewDashboard = await AuthService.HasPermissionAsync(user, FSHAction.View, FSHResource.Dashboard);
        _canViewRoles = await AuthService.HasPermissionAsync(user, FSHAction.View, FSHResource.Roles);
        _canViewUsers = await AuthService.HasPermissionAsync(user, FSHAction.View, FSHResource.Users);
        _canViewTenants = await AuthService.HasPermissionAsync(user, FSHAction.View, FSHResource.Tenants);

        _canViewTickets = await AuthService.HasPermissionAsync(user, FSHAction.Create, FSHResource.Tickets);
    }
}