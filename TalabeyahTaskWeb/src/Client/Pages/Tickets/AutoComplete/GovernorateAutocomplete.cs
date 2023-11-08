using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using TalabeyahTaskWeb.Client.Infrastructure.ApiClient;
using TalabeyahTaskWeb.Client.Shared;

namespace TalabeyahTaskWeb.Client.Pages.Tickets;
public class GovernorateAutocomplete : MudAutocomplete<int>
{
    [Inject]
    private IStringLocalizer<GovernorateAutocomplete> L { get; set; } = default!;
    [Inject]
    private IGovernorateClient GovernorateClient { get; set; } = default!;
    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    private List<GovernorateDto> _Governorates = new();

    // supply default parameters, but leave the possibility to override them
    public override Task SetParametersAsync(ParameterView parameters)
    {
        Label = L["Governorates "];
        Variant = Variant.Filled;
        Dense = true;
        Margin = Margin.Dense;
        ResetValueOnEmptyText = true;
        SearchFunc = SearchGovernorate;
        ToStringFunc = GetGovernorateName;
        Clearable = true;
        return base.SetParametersAsync(parameters);
    }

    // when the value parameter is set, we have to load that one brand to be able to show the name
    // we can't do that in OnInitialized because of a strange bug (https://github.com/MudBlazor/MudBlazor/issues/3818)
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender &&
            _value != default &&
            await ApiHelper.ExecuteCallGuardedAsync(
                () => GovernorateClient.GetAsync(_value), Snackbar) is { } Governorate)
        {
            _Governorates.Add(Governorate);
            ForceRender(true);
        }
    }

    private async Task<IEnumerable<int>> SearchGovernorate(string value)
    {
        var filter = new SearchGovernorateRequest
        {
            PageSize = 10,
            AdvancedSearch = new() { Fields = new[] { "name" }, Keyword = value }
        };

        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => GovernorateClient.SearchAdminAsync(filter), Snackbar)
            is PaginationResponseOfGovernorateDto response)
        {
            _Governorates = response.Data.ToList();
        }

        return _Governorates.Select(x => x.Id);
    }

    private string GetGovernorateName(int id) =>
        _Governorates.Find(b => b.Id == id)?.Name ?? string.Empty;
}