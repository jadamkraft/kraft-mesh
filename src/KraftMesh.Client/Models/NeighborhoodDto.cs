using System.Text.Json.Serialization;

namespace KraftMesh.Client.Models;

/// <summary>
/// Neighborhood (tenant) for dropdown. Matches API GET /neighborhoods response (camelCase).
/// </summary>
public sealed record NeighborhoodDto(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("name")] string Name);
