namespace KraftMesh.Infrastructure;

/// <summary>
/// Seeds development data. Called during Development startup.
/// </summary>
public interface IDbInitializer
{
    Task SeedAsync(CancellationToken ct = default);
}
