namespace KraftMesh.Core.Entities;

/// <summary>
/// User profile within a neighborhood (tenant). Implements ITenantEntity.
/// </summary>
public class Resident : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string? UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
