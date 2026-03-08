namespace KraftMesh.Core.Entities;

/// <summary>
/// Have/Need record within a neighborhood. Implements ITenantEntity.
/// </summary>
public class ResourceEntry : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public bool IsHave { get; set; }
    public ResourceCategory Category { get; set; }
    public ResourceSeverity Severity { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
