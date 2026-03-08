using KraftMesh.Core;
using Microsoft.Extensions.Options;

namespace KraftMesh.Infrastructure.Services;

/// <summary>
/// Scoped implementation of ITenantProvider. Resolves tenant from override (e.g. X-Tenant-Id or seed) first, then config.
/// </summary>
public sealed class TenantProvider : ITenantProvider
{
    private readonly ITenantIdOverride _override;
    private readonly Guid? _configTenantId;

    public TenantProvider(IOptions<TenantOptions> options, ITenantIdOverride overrideService)
    {
        _override = overrideService;
        var opts = options.Value;
        _configTenantId = Guid.TryParse(opts.DefaultTenantId, out var id) ? id : null;
    }

    public Guid? TenantId => _override.TenantId ?? _configTenantId;
}

/// <summary>
/// Options for tenant resolution (e.g. default tenant for development).
/// </summary>
public sealed class TenantOptions
{
    public const string SectionName = "KraftMesh:Tenant";

    public string? DefaultTenantId { get; set; }
}
