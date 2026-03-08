namespace KraftMesh.Infrastructure.Services;

/// <summary>
/// Scoped service that allows overriding the current tenant for the request (e.g. from X-Tenant-Id header or during seed).
/// TenantProvider resolves tenant from override first, then config.
/// </summary>
public interface ITenantIdOverride
{
    /// <summary>
    /// When set, this tenant ID is used for the current scope instead of config default.
    /// </summary>
    Guid? TenantId { get; }

    /// <summary>
    /// Sets the tenant override for the current scope.
    /// </summary>
    void Set(Guid tenantId);
}
