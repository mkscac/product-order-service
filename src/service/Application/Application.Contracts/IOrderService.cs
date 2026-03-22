using Application.Abstractions.Persistence;
using Application.Models.Orders;

namespace Application.Contracts;

public interface IOrderService
{
    Task<long> CreateAsync(string createdBy, CancellationToken ct);

    Task<long> AddProductAsync(
        long orderId,
        long productId,
        int quantity,
        CancellationToken ct);

    Task DeleteProductAsync(long orderId, long productId, CancellationToken ct);

    Task ExecutionOrderAsync(long orderId, CancellationToken ct);

    Task CompletedOrderAsync(long orderId, CancellationToken ct);

    Task CancelOrderAsync(long orderId, CancellationToken ct);

    Task<Page<OrderHistory>> SearchHistoryAsync(
        long orderId,
        PaginationParams paginationParams,
        CancellationToken ct);
}