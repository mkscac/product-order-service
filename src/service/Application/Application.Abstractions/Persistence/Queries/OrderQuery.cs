using Application.Models.Orders;

namespace Application.Abstractions.Persistence.Queries;

public sealed record OrderQuery(
    IReadOnlyList<long>? Ids,
    OrderState? State,
    string? CreatedBy);