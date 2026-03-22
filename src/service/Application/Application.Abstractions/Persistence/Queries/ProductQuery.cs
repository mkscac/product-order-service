namespace Application.Abstractions.Persistence.Queries;

public sealed record ProductQuery(
    IReadOnlyList<long>? Ids,
    decimal? MinPrice,
    decimal? MaxPrice,
    string? NameSubstring);