namespace KraftMesh.Infrastructure.Services;

/// <summary>
/// Scoped implementation of ITenantIdOverride. Middleware or seed code sets the tenant per request/scope.
/// </summary>
public sealed class TenantIdOverride : ITenantIdOverride
{
    public Guid? TenantId { get; private set; }

    public void Set(Guid tenantId) => TenantId = tenantId;
}
