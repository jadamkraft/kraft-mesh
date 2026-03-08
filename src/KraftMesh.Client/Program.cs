using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using KraftMesh.Client;
using KraftMesh.Client.Services;
using Polly;
using Polly.Extensions.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
    ?? builder.Configuration["services__api__http__0"]
    ?? builder.HostEnvironment.BaseAddress;

builder.Services.AddHttpClient("KraftMesh.Api", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl.TrimEnd('/') + "/");
})
.AddPolicyHandler(GetRetryPolicy());

builder.Services.AddScoped<ILocalVaultStorage, LocalVaultStorage>();
builder.Services.AddScoped<IResourceService>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = factory.CreateClient("KraftMesh.Api");
    return new ResourceService(
        httpClient,
        sp.GetRequiredService<ILocalVaultStorage>(),
        sp.GetRequiredService<ILogger<ResourceService>>());
});

await builder.Build().RunAsync();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}
