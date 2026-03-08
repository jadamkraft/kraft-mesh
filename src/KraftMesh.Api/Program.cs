using KraftMesh.Core;
using KraftMesh.Core.Entities;
using KraftMesh.Infrastructure;
using KraftMesh.Infrastructure.Interceptors;
using KraftMesh.Infrastructure.Services;
using KraftMesh.Api.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.Configure<TenantOptions>(builder.Configuration.GetSection(TenantOptions.SectionName));
builder.Services.AddScoped<ITenantIdOverride, TenantIdOverride>();
builder.Services.AddScoped<ITenantProvider, TenantProvider>();
builder.Services.AddScoped<TenantInterceptor>();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    var connectionString = builder.Configuration.GetConnectionString("database")
        ?? throw new InvalidOperationException("Connection string 'database' not found.");
    options.UseSqlServer(connectionString);
    options.AddInterceptors(sp.GetRequiredService<TenantInterceptor>());
});
builder.EnrichSqlServerDbContext<AppDbContext>();

var app = builder.Build();

app.Use(async (context, next) =>
{
    if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var value) &&
        Guid.TryParse(value, out var tenantId))
    {
        var overrideService = context.RequestServices.GetRequiredService<ITenantIdOverride>();
        overrideService.Set(tenantId);
    }
    await next(context);
});

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
    }
    using (var scope = app.Services.CreateScope())
    {
        var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        await initializer.SeedAsync();
    }
}

app.MapDefaultEndpoints();

app.MapGet("/", () => Results.Ok("KraftMesh.Api"));

app.MapGet("/neighborhoods", async (AppDbContext db, CancellationToken ct) =>
{
    var list = await db.Neighborhoods
        .OrderBy(n => n.Name)
        .Select(n => new { n.Id, n.Name })
        .ToListAsync(ct);
    return Results.Ok(list);
});

app.MapGet("/resources", async (AppDbContext db, CancellationToken ct) =>
{
    var list = await db.ResourceEntries.ToListAsync(ct);
    return Results.Ok(list);
});

app.MapPost("/resources", async (CreateResourceRequest req, AppDbContext db, CancellationToken ct) =>
{
    var entry = new ResourceEntry
    {
        Id = Guid.NewGuid(),
        IsHave = req.IsHave,
        Category = req.Category,
        Severity = req.Severity,
        Title = req.Title ?? string.Empty,
        Description = req.Description,
        CreatedAt = DateTimeOffset.UtcNow
    };
    db.ResourceEntries.Add(entry);
    await db.SaveChangesAsync(ct);
    return Results.Created($"/resources/{entry.Id}", entry);
});

app.Run();
