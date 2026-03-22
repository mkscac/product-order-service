using HttpGateway.Models.Orders.History;

namespace HttpGateway.Models;

public interface IOrderServiceGrpcClient
{
    Task<long> CreateOrderAsync(string createdBy, CancellationToken ct);

    Task<long> AddProductInOrderAsync(
        long orderId,
        long productId,
        int quantity,
        CancellationToken ct);

    Task DeleteProductFromOrderAsync(long orderId, long productId, CancellationToken ct);

    Task ExecutionOrderAsync(long orderId, CancellationToken ct);

    Task CancelOrderAsync(long orderId, CancellationToken ct);

    Task<IEnumerable<OrderHistoryDto>> SearchOrderHistoryAsync(
        long orderId,
        int pageNumber,
        int pageSize,
        CancellationToken ct);
}