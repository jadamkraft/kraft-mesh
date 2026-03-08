using KraftMesh.Core.Entities;

namespace KraftMesh.Api.Models;

/// <summary>
/// Request body for creating a new resource entry (Have or Need).
/// </summary>
public sealed record CreateResourceRequest(
    bool IsHave,
    ResourceCategory Category,
    ResourceSeverity Severity,
    string Title,
    string? Description);
