using Application.Abstractions.Persistence.Queries;
using Application.Models.Orders;

namespace Application.Abstractions.Persistence.Repositories;

public interface IOrderItemRepository
{
    Task<long> AddAsync(long orderId, long productId, int quantity, CancellationToken ct);

    Task DeleteAsync(long orderId, long productId, CancellationToken ct);

    Task<Page<OrderItem>> SearchAsync(
        OrderItemQuery query,
        PaginationParams parameters,
        CancellationToken ct);
}