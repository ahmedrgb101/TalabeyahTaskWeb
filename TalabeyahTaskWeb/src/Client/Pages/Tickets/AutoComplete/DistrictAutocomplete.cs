using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using TalabeyahTaskWeb.Client.Infrastructure.ApiClient;
using TalabeyahTaskWeb.Client.Shared;

namespace TalabeyahTaskWeb.Client.Pages.Tickets;
public class DistrictAutocomplete : MudAutocomplete<int>
{
    [Inject]
    private IStringLocalizer<DistrictAutocomplete> L { get; set; } = default!;
    [Inject]
    private IDistrictClient DistrictClient { get; set; } = default!;
    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    private List<DistrictDto> _Districts = new();

    // supply default parameters, but leave the possibility to override them
    public override Task SetParametersAsync(ParameterView parameters)
    {
        Label = L["Districts"];
        Variant = Variant.Filled;
        Dense = true;
        Margin = Margin.Dense;
        ResetValueOnEmptyText = true;
        SearchFunc = SearchDistrict;
        ToStringFunc = GetDistrictName;
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
                () => DistrictClient.GetAsync(_value), Snackbar) is { } District)
        {
            _Districts.Add(District);
            ForceRender(true);
        }
    }

    private async Task<IEnumerable<int>> SearchDistrict(string value)
    {
        var filter = new SearchDistrictRequest
        {
            PageSize = 10,
            AdvancedSearch = new() { Fields = new[] { "name" }, Keyword = value }
        };

        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => DistrictClient.SearchAdminAsync(filter), Snackbar)
            is PaginationResponseOfDistrictDto response)
        {
            _Districts = response.Data.ToList();
        }

        return _Districts.Select(x => x.Id);
    }

    private string GetDistrictName(int id) =>
        _Districts.Find(b => b.Id == id)?.Name ?? string.Empty;
}