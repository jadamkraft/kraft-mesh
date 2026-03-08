using KraftMesh.Core;
using KraftMesh.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace KraftMesh.Infrastructure.Interceptors;

/// <summary>
/// Sets TenantId on all added entities that implement ITenantEntity, from the current ITenantProvider.
/// </summary>
public sealed class TenantInterceptor : SaveChangesInterceptor
{
    private readonly ITenantProvider _tenantProvider;

    public TenantInterceptor(ITenantProvider tenantProvider)
    {
        _tenantProvider = tenantProvider;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
            SetTenantIdOnAdded(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
            SetTenantIdOnAdded(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void SetTenantIdOnAdded(DbContext context)
    {
        var tenantId = _tenantProvider.TenantId;
        if (tenantId is null)
            return;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.State != EntityState.Added)
                continue;
            if (entry.Entity is not ITenantEntity tenantEntity)
                continue;
            entry.Property(nameof(ITenantEntity.TenantId)).CurrentValue = tenantId.Value;
        }
    }
}
