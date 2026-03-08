namespace KraftMesh.Core.Entities;

/// <summary>
/// Marks an entity as belonging to a tenant. All tenant-scoped entities implement this.
/// </summary>
public interface ITenantEntity
{
    Guid TenantId { get; }
}
