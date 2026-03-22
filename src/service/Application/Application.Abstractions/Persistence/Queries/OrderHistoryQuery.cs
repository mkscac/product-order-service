using Application.Models.Orders;

namespace Application.Abstractions.Persistence.Queries;

public sealed record OrderHistoryQuery(
    IReadOnlyList<long>? OrderIds,
    OrderHistoryItemKind? ItemKind);