using KraftMesh.Core;
using KraftMesh.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace KraftMesh.Infrastructure;

public sealed class AppDbContext : DbContext
{
    private readonly ITenantProvider _tenantProvider;

    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantProvider tenantProvider)
        : base(options)
    {
        _tenantProvider = tenantProvider;
    }

    public DbSet<Neighborhood> Neighborhoods => Set<Neighborhood>();
    public DbSet<Resident> Residents => Set<Resident>();
    public DbSet<ResourceEntry> ResourceEntries => Set<ResourceEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Neighborhood>(b =>
        {
            b.HasKey(e => e.Id);
            b.Property(e => e.Name).HasMaxLength(256);
        });

        modelBuilder.Entity<Resident>(b =>
        {
            b.HasKey(e => e.Id);
            b.Property(e => e.TenantId);
            b.Property(e => e.UserId).HasMaxLength(256);
            b.Property(e => e.DisplayName).HasMaxLength(256);
            b.Property(e => e.Email).HasMaxLength(256);
            b.HasOne<Neighborhood>()
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasIndex(e => e.TenantId);
            b.HasQueryFilter(e => e.TenantId == _tenantProvider.TenantId);
        });

        modelBuilder.Entity<ResourceEntry>(b =>
        {
            b.HasKey(e => e.Id);
            b.Property(e => e.TenantId);
            b.Property(e => e.Title).HasMaxLength(512);
            b.Property(e => e.Description).HasMaxLength(2048);
            b.HasOne<Neighborhood>()
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasIndex(e => new { e.TenantId, e.Category });
            b.HasQueryFilter(e => e.TenantId == _tenantProvider.TenantId);
        });
    }
}
