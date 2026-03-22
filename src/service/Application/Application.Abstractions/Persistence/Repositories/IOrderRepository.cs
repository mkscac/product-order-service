using Application.Abstractions.Persistence.Queries;
using Application.Models.Orders;

namespace Application.Abstractions.Persistence.Repositories;

public interface IOrderRepository
{
    Task<long> CreateAsync(string createdBy, CancellationToken ct);

    Task ChangeStateAsync(long id, OrderState toState, CancellationToken ct);

    Task<Page<Order>> SearchAsync(
        OrderQuery query,
        PaginationParams parameters,
        CancellationToken ct);
}