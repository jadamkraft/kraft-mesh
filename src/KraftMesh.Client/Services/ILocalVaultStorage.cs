using KraftMesh.Core.Entities;

namespace KraftMesh.Client.Services;

/// <summary>
/// Local Vault (IndexedDB) storage for offline cache of resource entries.
/// </summary>
public interface ILocalVaultStorage
{
    /// <summary>
    /// Gets the cached list of resources from IndexedDB, or null if missing or invalid.
    /// </summary>
    Task<List<ResourceEntry>?> GetResourcesAsync(CancellationToken ct = default);

    /// <summary>
    /// Persists the list of resources to IndexedDB.
    /// </summary>
    Task SetResourcesAsync(IReadOnlyList<ResourceEntry> resources, CancellationToken ct = default);
}
