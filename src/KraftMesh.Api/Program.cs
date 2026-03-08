using KraftMesh.Core;
using KraftMesh.Infrastructure;
using KraftMesh.Infrastructure.Interceptors;
using KraftMesh.Infrastructure.Services;
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

app.Run();
