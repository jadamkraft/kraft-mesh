using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace KraftMesh.Infrastructure;

/// <summary>
/// Used by EF Core tools at design time (e.g. migrations) when the startup project is the Api.
/// When running with Api as startup, connection string comes from Api's config.
/// This factory is used when the startup project is Infrastructure (no app host).
/// </summary>
public sealed class DesignTimeAppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();
        if (basePath.EndsWith("KraftMesh.Infrastructure", StringComparison.OrdinalIgnoreCase))
            basePath = Path.Combine(basePath, "..", "KraftMesh.Api");
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("database")
            ?? "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=KraftMesh;Integrated Security=True;";

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        var tenantProvider = new DesignTimeTenantProvider();
        return new AppDbContext(optionsBuilder.Options, tenantProvider);
    }

    private sealed class DesignTimeTenantProvider : Core.ITenantProvider
    {
        public Guid? TenantId => Guid.Empty;
    }
}
