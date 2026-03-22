using System.Text.Json.Serialization;

namespace Configuration.Models;

public sealed record ConfigurationPage(
    [property: JsonPropertyName("items")] IReadOnlyList<ConfigurationItem> Items,
    [property: JsonPropertyName("pageToken")] string? PageToken);