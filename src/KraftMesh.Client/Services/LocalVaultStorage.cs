using System.Text.Json;
using KraftMesh.Core.Entities;
using Microsoft.JSInterop;

namespace KraftMesh.Client.Services;

/// <summary>
/// JS Interop-backed IndexedDB storage for the Local Vault cache.
/// </summary>
public sealed class LocalVaultStorage : ILocalVaultStorage
{
    private const string ResourcesKey = "resources";
    private readonly IJSRuntime _jsRuntime;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public LocalVaultStorage(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <inheritdoc />
    public async Task<List<ResourceEntry>?> GetResourcesAsync(CancellationToken ct = default)
    {
        try
        {
            var json = await _jsRuntime.InvokeAsync<string?>(
                "KraftMeshStorage_getItem",
                ct,
                ResourcesKey);

            if (string.IsNullOrWhiteSpace(json))
                return null;

            var list = JsonSerializer.Deserialize<List<ResourceEntry>>(json, JsonOptions);
            return list ?? null;
        }
        catch (JSException)
        {
            return null;
        }
    }

    /// <inheritdoc />
    public async Task SetResourcesAsync(IReadOnlyList<ResourceEntry> resources, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(resources, JsonOptions);
        await _jsRuntime.InvokeVoidAsync(
            "KraftMeshStorage_setItem",
            ct,
            ResourcesKey,
            json);
    }
}
