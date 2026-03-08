namespace KraftMesh.Core.Entities;

/// <summary>
/// Represents a tenant (neighborhood) in the system. Does not implement ITenantEntity as it is the tenant root.
/// </summary>
public class Neighborhood
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}
