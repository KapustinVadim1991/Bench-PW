using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace WebComponents.Components.SelectBox;

public partial class SelectBox<TItem>
{
    #region Component Parameters

    /// <summary>
    /// Local list of items (used for standard select and local filtering)
    /// </summary>
    [Parameter]
    public IEnumerable<TItem>? Items { get; set; }

    /// <summary>
    /// Enables autocomplete mode (displays an input field for filtering)
    /// </summary>
    [Parameter]
    public bool Autocomplete { get; set; }

    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Delegate for implementing local collection search.
    /// The current filter value is passed when invoked.
    /// </summary>
    [Parameter]
    public Func<string?, Task<IEnumerable<TItem>>>? OnSearch { get; set; }

    /// <summary>
    /// Delegate for server-side data loading.
    /// A request object with pagination and filter parameters is passed when invoked.
    /// </summary>
    [Parameter]
    public Func<ServerSideRequest, Task<ServerSideResponse<TItem>>>? OnServerSideSearch { get; set; }

    /// <summary>
    /// Template for displaying an item.
    /// </summary>
    [Parameter]
    public RenderFragment<TItem>? ItemTemplate { get; set; }

    /// <summary>
    /// Placeholder text displayed when no item is selected.
    /// </summary>
    [Parameter]
    public string Placeholder { get; set; } = "Select...";

    /// <summary>
    /// Page size for server-side loading (number of items per request).
    /// </summary>
    [Parameter]
    public int ServerSidePageSize { get; set; } = 10;

    /// <summary>
    /// Bound value of the selected item.
    /// </summary>
    [Parameter]
    public TItem? Value { get; set; }

    /// <summary>
    /// Event callback for when the bound value changes.
    /// </summary>
    [Parameter]
    public EventCallback<TItem?> ValueChanged { get; set; }

    /// <summary>
    /// Expression for the bound value (optional, used for validation).
    /// </summary>
    [Parameter]
    public Expression<Func<TItem>>? ValueExpression { get; set; }

    #endregion

    #region Local Variables

    private bool _isOpen;
    private string? _filterText;
    private List<TItem> _displayedItems = new();
    private TItem? _selectedItem;
    private bool _isLoading;
    private int _serverSideCurrentIndex;
    private int _totalCount;
    private bool _hasMoreData = true;
    private readonly string _selectBoxId = $"selectbox_{Guid.NewGuid()}";
    private readonly string _dropdownId = $"dropdown_{Guid.NewGuid()}";
    private readonly string _dropdownItemsId = $"dropdownItems_{Guid.NewGuid()}";
    private readonly string _dropdownInputFilterId = $"dropdownFilter_{Guid.NewGuid()}";
    private DotNetObjectReference<SelectBox<TItem>> _dotNetObjectReference = null!;

    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;

    #endregion

    #region Lifecycle and Rendering Methods

    private bool _initializedDropdown;

    protected override void OnParametersSet()
    {
        // Update the selected item from the bound value if provided
        if (Value != null)
        {
            _selectedItem = Value;
        }
    }

    /// <summary>
    /// After rendering, if the dropdown is open, call JS to position the dropdown list.
    /// </summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _dotNetObjectReference = DotNetObjectReference.Create(this);
        }

        if (_isOpen)
        {
            if (!_initializedDropdown)
            {
                _initializedDropdown = true;
                await JsRuntime.InvokeVoidAsync("_selectLib.addScrollDropdownEventListeners", _dropdownItemsId,
                    _dotNetObjectReference);
                await JsRuntime.InvokeVoidAsync("focusElement", _dropdownInputFilterId);
            }

            await JsRuntime.InvokeVoidAsync("_selectLib.positionDropdown", _selectBoxId, _dropdownId, _dropdownItemsId);
        }

        if (!_isOpen)
        {
            _initializedDropdown = false;
        }
    }

    /// <summary>
    /// Toggles the dropdown state.
    /// When opened, resets the filter and starts data loading.
    /// </summary>
    private async Task ToggleDropdown()
    {
        _isOpen = !_isOpen;
        if (_isOpen)
        {
            _filterText = null;
            _serverSideCurrentIndex = 0;
            _hasMoreData = true;
            await LoadData();
        }
        else
        {
            await JsRuntime.InvokeVoidAsync("_selectLib.resetScrollDropdownEventListeners", _dropdownItemsId);
        }
    }

    [JSInvokable]
    public async Task LoadData(bool append = false)
    {
        if (!_hasMoreData)
        {
            return;
        }

        if (OnServerSideSearch != null)
        {
            _isLoading = true;
            StateHasChanged();
            var request = new ServerSideRequest
            {
                StartIndex = _serverSideCurrentIndex,
                Count = ServerSidePageSize,
                Filter = _filterText
            };
            var result = await OnServerSideSearch(request);
            if (append)
            {
                _displayedItems.AddRange(result.Items);
            }
            else
            {
                _displayedItems = result.Items.ToList();
            }

            _serverSideCurrentIndex += ServerSidePageSize;
            _totalCount = result.TotalCount;

            if (_displayedItems.Count >= _totalCount)
            {
                await JsRuntime.InvokeVoidAsync("_selectLib.resetScrollDropdownEventListeners", _dropdownItemsId);
                _hasMoreData = false;
            }

            _isLoading = false;
            StateHasChanged();
        }
        else if (Autocomplete && OnSearch != null)
        {
            _isLoading = true;
            StateHasChanged();
            var result = await OnSearch(_filterText);
            _displayedItems = result.ToList();
            _isLoading = false;
            StateHasChanged();
        }
    }

    /// <summary>
    /// Filter change handler.
    /// Reloads data when the filter changes.
    /// </summary>
    private async Task OnFilterChanged(ChangeEventArgs e)
    {
        _filterText = e.Value?.ToString() ?? "";
        _serverSideCurrentIndex = 0;

        if (!_hasMoreData)
        {
            await JsRuntime.InvokeVoidAsync("_selectLib.addScrollDropdownEventListeners", _dropdownItemsId,
                _dotNetObjectReference);
            _hasMoreData = true;
        }

        await LoadData();
    }

    /// <summary>
    /// Selects an item from the dropdown.
    /// Closes the dropdown, updates the bound value, and invokes the selection callback.
    /// </summary>
    private async Task SelectItem(TItem item)
    {
        _selectedItem = item;
        _isOpen = false;

        // Update the bound value and notify the parent component
        Value = item;
        await ValueChanged.InvokeAsync(item);
    }

    #endregion
}