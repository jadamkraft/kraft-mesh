using KraftMesh.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KraftMesh.Infrastructure;

/// <summary>
/// Seeds development data: Sapulpa and Glenpool neighborhoods with their resource entries.
/// Idempotent: safe to run on every Development startup.
/// </summary>
public sealed class DbInitializer : IDbInitializer
{
    private static readonly Guid SapulpaId = new("11111111-1111-1111-1111-111111111111");
    private const string SapulpaName = "Sapulpa";
    private static readonly Guid GlenpoolId = new("22222222-2222-2222-2222-222222222222");
    private const string GlenpoolName = "Glenpool";

    private readonly IServiceScopeFactory _scopeFactory;

    public DbInitializer(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        await EnsureNeighborhoodsAsync(ct).ConfigureAwait(false);
        await SeedSapulpaResourcesAsync(ct).ConfigureAwait(false);
        await SeedGlenpoolResourcesAsync(ct).ConfigureAwait(false);
    }

    private async Task EnsureNeighborhoodsAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var existing = await db.Neighborhoods
            .Where(n => n.Id == SapulpaId || n.Id == GlenpoolId)
            .Select(n => n.Id)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        var now = DateTimeOffset.UtcNow;
        if (!existing.Contains(SapulpaId))
        {
            db.Neighborhoods.Add(new Neighborhood { Id = SapulpaId, Name = SapulpaName, CreatedAt = now });
        }
        if (!existing.Contains(GlenpoolId))
        {
            db.Neighborhoods.Add(new Neighborhood { Id = GlenpoolId, Name = GlenpoolName, CreatedAt = now });
        }
        if (existing.Count < 2)
        {
            await db.SaveChangesAsync(ct).ConfigureAwait(false);
        }
    }

    private async Task SeedSapulpaResourcesAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var overrideService = scope.ServiceProvider.GetRequiredService<Services.ITenantIdOverride>();
        overrideService.Set(SapulpaId);

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var existingCount = await db.ResourceEntries.CountAsync(ct).ConfigureAwait(false);
        if (existingCount > 0)
            return;

        var now = DateTimeOffset.UtcNow;
        db.ResourceEntries.AddRange(
            new ResourceEntry
            {
                Id = Guid.NewGuid(),
                IsHave = false,
                Category = ResourceCategory.Goods,
                Severity = ResourceSeverity.Critical,
                Title = "Medical Oxygen Power",
                CreatedAt = now
            },
            new ResourceEntry
            {
                Id = Guid.NewGuid(),
                IsHave = false,
                Category = ResourceCategory.Goods,
                Severity = ResourceSeverity.High,
                Title = "Water Filters",
                CreatedAt = now
            });
        await db.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    private async Task SeedGlenpoolResourcesAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var overrideService = scope.ServiceProvider.GetRequiredService<Services.ITenantIdOverride>();
        overrideService.Set(GlenpoolId);

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var existingCount = await db.ResourceEntries.CountAsync(ct).ConfigureAwait(false);
        if (existingCount > 0)
            return;

        var now = DateTimeOffset.UtcNow;
        db.ResourceEntries.AddRange(
            new ResourceEntry
            {
                Id = Guid.NewGuid(),
                IsHave = true,
                Category = ResourceCategory.Goods,
                Severity = ResourceSeverity.Low,
                Title = "Solar Generator",
                CreatedAt = now
            },
            new ResourceEntry
            {
                Id = Guid.NewGuid(),
                IsHave = true,
                Category = ResourceCategory.Tools,
                Severity = ResourceSeverity.Low,
                Title = "Chain Saw",
                CreatedAt = now
            });
        await db.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}
