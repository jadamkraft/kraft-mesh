using System.Net.Http.Json;
using KraftMesh.Client.Models;
using Microsoft.Extensions.Logging;

namespace KraftMesh.Client.Services;

/// <summary>
/// Holds the list of neighborhoods and the current selected tenant for the UI.
/// Sends X-Tenant-Id on API requests when set. Load neighborhoods from GET /neighborhoods.
/// </summary>
public sealed class TenantState
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TenantState> _logger;
    private Guid? _currentTenantId;

    public TenantState(IHttpClientFactory httpClientFactory, ILogger<TenantState> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public IReadOnlyList<NeighborhoodDto> Neighborhoods { get; private set; } = Array.Empty<NeighborhoodDto>();

    public Guid? CurrentTenantId
    {
        get => _currentTenantId;
        set => _currentTenantId = value;
    }

    public event Action? OnChange;

    public async Task LoadNeighborhoodsAsync(CancellationToken ct = default)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("KraftMesh.Api");
            var list = await client.GetFromJsonAsync<List<NeighborhoodDto>>("neighborhoods", ct).ConfigureAwait(false);
            Neighborhoods = list ?? new List<NeighborhoodDto>();
            if (Neighborhoods.Count > 0 && _currentTenantId is null)
            {
                _currentTenantId = Neighborhoods[0].Id;
            }
            NotifyStateChanged();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load neighborhoods");
            Neighborhoods = Array.Empty<NeighborhoodDto>();
            NotifyStateChanged();
        }
    }

    public void SetTenant(Guid tenantId)
    {
        if (_currentTenantId == tenantId)
            return;
        _currentTenantId = tenantId;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
