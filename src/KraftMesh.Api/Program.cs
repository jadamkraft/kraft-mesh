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
builder.Services.AddScoped<ITenantProvider, TenantProvider>();
builder.Services.AddScoped<TenantInterceptor>();
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    var connectionString = builder.Configuration.GetConnectionString("database")
        ?? throw new InvalidOperationException("Connection string 'database' not found.");
    options.UseSqlServer(connectionString);
    options.AddInterceptors(sp.GetRequiredService<TenantInterceptor>());
});
builder.EnrichSqlServerDbContext<AppDbContext>();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/", () => Results.Ok("KraftMesh.Api"));

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
