using System.Net.Http.Json;
using KraftMesh.Core.Entities;
using Microsoft.Extensions.Logging;

namespace KraftMesh.Client.Services;

/// <summary>
/// Resource service with Polly-retried API calls and automatic fallback to Local Vault (IndexedDB).
/// </summary>
public sealed class ResourceService : IResourceService
{
    private const string ResourcesPath = "resources";
    private readonly HttpClient _httpClient;
    private readonly ILocalVaultStorage _localVault;
    private readonly ILogger<ResourceService> _logger;
    private bool _isOffline;

    public ResourceService(
        HttpClient httpClient,
        ILocalVaultStorage localVault,
        ILogger<ResourceService> logger)
    {
        _httpClient = httpClient;
        _localVault = localVault;
        _logger = logger;
    }

    public bool IsOffline => _isOffline;

    public async Task<List<ResourceEntry>> GetResourcesAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(ResourcesPath, ct);
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadFromJsonAsync<List<ResourceEntry>>(ct) ?? new List<ResourceEntry>();
            _isOffline = false;
            await _localVault.SetResourcesAsync(list, ct);
            return list;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "API get resources failed; falling back to Local Vault");
            _isOffline = true;
            var cached = await _localVault.GetResourcesAsync(ct);
            return cached ?? new List<ResourceEntry>();
        }
    }

    public async Task<ResourceEntry?> AddResourceAsync(CreateResourceDto dto, CancellationToken ct = default)
    {
        try
        {
            var payload = new
            {
                dto.IsHave,
                dto.Category,
                dto.Severity,
                dto.Title,
                dto.Description
            };
            var response = await _httpClient.PostAsJsonAsync(ResourcesPath, payload, ct);
            response.EnsureSuccessStatusCode();
            var created = await response.Content.ReadFromJsonAsync<ResourceEntry>(ct);
            if (created is not null)
                await GetResourcesAsync(ct);
            return created;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "API add resource failed");
            return null;
        }
    }
}
