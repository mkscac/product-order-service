namespace Application.Abstractions.Persistence;

public sealed record Page<T>(IReadOnlyList<T> Items);