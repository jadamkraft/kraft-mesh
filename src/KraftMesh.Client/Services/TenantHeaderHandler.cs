using System.Net.Http.Headers;

namespace KraftMesh.Client.Services;

/// <summary>
/// Adds X-Tenant-Id to outgoing API requests when TenantState has a current tenant.
/// </summary>
public sealed class TenantHeaderHandler : DelegatingHandler
{
    private readonly TenantState _tenantState;

    public TenantHeaderHandler(TenantState tenantState)
    {
        _tenantState = tenantState;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_tenantState.CurrentTenantId is { } tenantId)
        {
            request.Headers.TryAddWithoutValidation("X-Tenant-Id", tenantId.ToString());
        }
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
