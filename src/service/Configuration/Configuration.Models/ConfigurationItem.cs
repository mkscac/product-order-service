using System.Text.Json.Serialization;

namespace Configuration.Models;

public sealed record ConfigurationItem(
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("value")] string Value);