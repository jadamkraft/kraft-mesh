namespace KraftMesh.Core;

/// <summary>
/// Provides the current tenant ID for the request. Implemented in Infrastructure; used by DbContext and interceptors.
/// </summary>
public interface ITenantProvider
{
    Guid? TenantId { get; }
}
