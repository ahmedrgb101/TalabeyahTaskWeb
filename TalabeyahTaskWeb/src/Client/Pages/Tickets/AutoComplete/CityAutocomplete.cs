using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using System.Linq.Expressions;
using TalabeyahTaskWeb.Client.Infrastructure.ApiClient;
using TalabeyahTaskWeb.Client.Shared;

namespace TalabeyahTaskWeb.Client.Pages.Tickets;
public class CityAutocomplete : MudAutocomplete<int>
{
    [Inject]
    private IStringLocalizer<CityAutocomplete> L { get; set; } = default!;
    [Inject]
    private ICityClient RegionClient { get; set; } = default!;
    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    public int _governorateId;

    /// <summary>
    /// The value of this input element.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.FormComponent.Data)]
    public int GovernorateId
    {
        get => _governorateId;
        set => _governorateId = value;
    }

    private List<CityDto> _Cities = new();

    // supply default parameters, but leave the possibility to override them
    public override Task SetParametersAsync(ParameterView parameters)
    {
        Label = L["City"];
        Variant = Variant.Filled;
        Dense = true;
        Margin = Margin.Dense;
        ResetValueOnEmptyText = true;
        SearchFunc = SearchRegion;
        ToStringFunc = GetRegionName;
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
                () => RegionClient.GetAsync(_value), Snackbar) is { } Region)
        {
            _Cities.Add(Region);
            ForceRender(true);
        }
    }

    private async Task<IEnumerable<int>> SearchRegion(string value)
    {
        var filter = new SearchCityRequest
        {
            PageSize = 10,
            AdvancedSearch = new() { Fields = new[] { "name" }, Keyword = value },
            GovernorateId = GovernorateId
        };

        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => RegionClient.SearchAsync(filter), Snackbar)
            is PaginationResponseOfCityDto response)
        {
            _Cities = response.Data.ToList();
        }

        return _Cities.Select(x => x.Id);
    }

    private string GetRegionName(int id) =>
        _Cities.Find(b => b.Id == id)?.Name ?? string.Empty;
}