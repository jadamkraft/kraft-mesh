using KraftMesh.Core.Entities;

namespace KraftMesh.Client.Services;

/// <summary>
/// Service for fetching and creating resource entries (Haves/Needs) with API call and Local Vault fallback.
/// </summary>
public interface IResourceService
{
    /// <summary>
    /// True when the last fetch used the Local Vault (offline) instead of the API.
    /// </summary>
    bool IsOffline { get; }

    /// <summary>
    /// Gets all resources from the API, or from the Local Vault if the API is unreachable.
    /// </summary>
    Task<List<ResourceEntry>> GetResourcesAsync(CancellationToken ct = default);

    /// <summary>
    /// Creates a resource via the API. On success, cache is refreshed. Returns null if offline or request failed.
    /// </summary>
    Task<ResourceEntry?> AddResourceAsync(CreateResourceDto dto, CancellationToken ct = default);
}

/// <summary>
/// DTO for creating a resource (matches API CreateResourceRequest shape).
/// </summary>
public sealed record CreateResourceDto(
    bool IsHave,
    ResourceCategory Category,
    ResourceSeverity Severity,
    string Title,
    string? Description);
