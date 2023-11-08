using System.Collections.ObjectModel;

namespace FSH.WebApi.Shared.Authorization;
public static class FSHAction
{
    public const string View = nameof(View);
    public const string Search = nameof(Search);
    public const string Create = nameof(Create);
    public const string Update = nameof(Update);
    public const string CheckInOut = nameof(CheckInOut);
    public const string Delete = nameof(Delete);
    public const string Export = nameof(Export);
    public const string ApplyToTicket = nameof(ApplyToTicket);
    public const string Generate = nameof(Generate);
    public const string Clean = nameof(Clean);
    public const string UpgradeSubscription = nameof(UpgradeSubscription);
}

public static class FSHResource
{
    public const string Tenants = nameof(Tenants);
    public const string Dashboard = nameof(Dashboard);
    public const string Order = nameof(Order);
    public const string Hangfire = nameof(Hangfire);
    public const string Users = nameof(Users);
    public const string UserRoles = nameof(UserRoles);
    public const string Roles = nameof(Roles);
    public const string RoleClaims = nameof(RoleClaims);
    public const string Tickets = nameof(Tickets);
}

public static class ActionPlanAction
{
    public const string Auditor = nameof(Auditor);
    public const string KeyOwner = nameof(KeyOwner);
    public const string MBU = nameof(MBU);
    public const string ViewTaskHistory = nameof(ViewTaskHistory);
}

public static class FSHPermissions
{
    private static readonly FSHPermission[] _all = new FSHPermission[]
    {
        new("View Dashboard", FSHAction.View, FSHResource.Dashboard),
        new("View Hangfire", FSHAction.View, FSHResource.Hangfire),
        new("View Users", FSHAction.View, FSHResource.Users),
        new("Search Users", FSHAction.Search, FSHResource.Users),
        new("Create Users", FSHAction.Create, FSHResource.Users),
        new("Update Users", FSHAction.Update, FSHResource.Users),
        new("Delete Users", FSHAction.Delete, FSHResource.Users),
        new("Export Users", FSHAction.Export, FSHResource.Users),
        new("View UserRoles", FSHAction.View, FSHResource.UserRoles),
        new("Update UserRoles", FSHAction.Update, FSHResource.UserRoles),
        new("View Roles", FSHAction.View, FSHResource.Roles),
        new("Create Roles", FSHAction.Create, FSHResource.Roles),
        new("Update Roles", FSHAction.Update, FSHResource.Roles),
        new("Delete Roles", FSHAction.Delete, FSHResource.Roles),
        new("View RoleClaims", FSHAction.View, FSHResource.RoleClaims),
        new("Update RoleClaims", FSHAction.Update, FSHResource.RoleClaims),

        new("View Tickets", FSHAction.View, FSHResource.Tickets, IsBasic: true),
        new("Search Tickets", FSHAction.Search, FSHResource.Tickets, IsBasic: true),
        new("Create Tickets", FSHAction.Create, FSHResource.Tickets, IsEmployer: true),
        new("Update Tickets", FSHAction.Update, FSHResource.Tickets, IsEmployer: true),
        new("Delete Tickets", FSHAction.Delete, FSHResource.Tickets, IsEmployer: true),
        new("Export Tickets", FSHAction.Export, FSHResource.Tickets, IsEmployer: true),
        new("Apply To Ticket", FSHAction.ApplyToTicket, FSHResource.Tickets, IsApplicant: true),

        new("View Tenants", FSHAction.View, FSHResource.Tenants, IsRoot: true),
        new("Create Tenants", FSHAction.Create, FSHResource.Tenants, IsRoot: true),
        new("Update Tenants", FSHAction.Update, FSHResource.Tenants, IsRoot: true),
        new("Upgrade Tenant Subscription", FSHAction.UpgradeSubscription, FSHResource.Tenants, IsRoot: true)
};

    public static IReadOnlyList<FSHPermission> All { get; } = new ReadOnlyCollection<FSHPermission>(_all);
    public static IReadOnlyList<FSHPermission> Root { get; } = new ReadOnlyCollection<FSHPermission>(_all.Where(p => p.IsRoot).ToArray());
    public static IReadOnlyList<FSHPermission> Admin { get; } = new ReadOnlyCollection<FSHPermission>(_all.Where(p => !p.IsRoot).ToArray());
    public static IReadOnlyList<FSHPermission> Employer { get; } = new ReadOnlyCollection<FSHPermission>(_all.Where(p => p.IsEmployer).ToArray());
    public static IReadOnlyList<FSHPermission> Applicant { get; } = new ReadOnlyCollection<FSHPermission>(_all.Where(p => p.IsApplicant).ToArray());
    public static IReadOnlyList<FSHPermission> Basic { get; } = new ReadOnlyCollection<FSHPermission>(_all.Where(p => p.IsBasic).ToArray());
}

public record FSHPermission(string Description, string Action, string Resource, bool IsBasic = false, bool IsRoot = false, bool IsEmployer = false, bool IsApplicant = false)
{
    public string Name => NameFor(Action, Resource);
    public static string NameFor(string action, string resource) => $"Permissions.{resource}.{action}";
}