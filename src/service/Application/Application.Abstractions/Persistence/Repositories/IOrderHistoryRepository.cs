using Application.Abstractions.Persistence.Queries;
using Application.Models.Orders;
using Application.Models.Orders.OrderHistoryPayloads;

namespace Application.Abstractions.Persistence.Repositories;

public interface IOrderHistoryRepository
{
    Task<long> AddNoteAsync(
        long orderId,
        OrderHistoryItemKind itemKind,
        OrderHistoryPayloadBase payload,
        CancellationToken ct);

    Task<Page<OrderHistory>> SearchAsync(
        OrderHistoryQuery query,
        PaginationParams parameters,
        CancellationToken ct);
}