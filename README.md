# KraftMesh

.NET 10 Aspire solution using Clean Architecture.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Aspire workload/templates](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/install-tooling) (optional; AppHost uses SDK package reference)

## Solution structure

| Project | Description |
|--------|-------------|
| **KraftMesh.AppHost** | Aspire orchestrator (dashboard, project references) |
| **KraftMesh.ServiceDefaults** | Shared observability, resilience, service discovery |
| **KraftMesh.Api** | Minimal APIs backend |
| **KraftMesh.Client** | Blazor WebAssembly PWA frontend |
| **KraftMesh.Core** | Domain/data (no business logic yet) |

## Build and run

```bash
dotnet build kraftmesh.sln
aspire run
# or: dotnet run --project src/KraftMesh.AppHost
```

Then open the Aspire dashboard URL shown in the terminal and use the **api** and **client** resource endpoints.

## Notes

- **Client** does not reference ServiceDefaults (Blazor WASM cannot reference `Microsoft.AspNetCore.App`). API base URL and resilience can be configured via `appsettings` or Aspire service discovery when running under AppHost.
- **global.json** pins the SDK to 10.0.x; install .NET 10 or adjust `global.json` if using a different SDK.
