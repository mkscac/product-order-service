namespace Application.Abstractions.Persistence.Queries;

public sealed record OrderItemQuery(
    IReadOnlyList<long>? OrderIds,
    IReadOnlyList<long>? ProductIds,
    bool? IsDeleted);