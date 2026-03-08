using KraftMesh.Core;
using Microsoft.Extensions.Options;

namespace KraftMesh.Infrastructure.Services;

/// <summary>
/// Scoped implementation of ITenantProvider. For Phase 2 returns a default tenant from config or a dev fallback.
/// </summary>
public sealed class TenantProvider : ITenantProvider
{
    private readonly Guid? _tenantId;

    public TenantProvider(IOptions<TenantOptions> options)
    {
        var opts = options.Value;
        _tenantId = Guid.TryParse(opts.DefaultTenantId, out var id) ? id : null;
    }

    public Guid? TenantId => _tenantId;
}

/// <summary>
/// Options for tenant resolution (e.g. default tenant for development).
/// </summary>
public sealed class TenantOptions
{
    public const string SectionName = "KraftMesh:Tenant";

    public string? DefaultTenantId { get; set; }
}
