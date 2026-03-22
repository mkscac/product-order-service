using HttpGateway.Models;
using HttpGateway.Models.Orders.History;
using Orders.Contracts;

namespace HttpGateway.Clients.GrpcClients;

public class OrderServiceGrpcClient : IOrderServiceGrpcClient
{
    private readonly MainOrderService.MainOrderServiceClient _client;

    public OrderServiceGrpcClient(MainOrderService.MainOrderServiceClient client)
    {
        _client = client;
    }

    public async Task<long> CreateOrderAsync(string createdBy, CancellationToken ct)
    {
        CreateOrderResponse response = await _client.CreateOrderAsync(
            new CreateOrderRequest
            {
                CreatedBy = createdBy,
            },
            cancellationToken: ct);
        return response.OrderId;
    }

    public async Task<long> AddProductInOrderAsync(
        long orderId,
        long productId,
        int quantity,
        CancellationToken ct)
    {
        AddProductInOrderResponse response = await _client.AddProductInOrderAsync(
            new AddProductInOrderRequest
            {
                OrderId = orderId,
                ProductId = productId,
                Quantity = quantity,
            },
            cancellationToken: ct);
        return response.OrderItemId;
    }

    public async Task DeleteProductFromOrderAsync(long orderId, long productId, CancellationToken ct)
    {
        await _client.DeleteProductFromOrderAsync(
            new DeleteProductFromOrderRequest
            {
                OrderId = orderId,
                ProductId = productId,
            },
            cancellationToken: ct);
    }

    public async Task ExecutionOrderAsync(long orderId, CancellationToken ct)
    {
        await _client.ExecutionOrderAsync(
            new ExecutionOrderRequest
            {
                OrderId = orderId,
            },
            cancellationToken: ct);
    }

    public async Task CancelOrderAsync(long orderId, CancellationToken ct)
    {
        await _client.CancelOrderAsync(
            new CancelOrderRequest
            {
                OrderId = orderId,
            },
            cancellationToken: ct);
    }

    public async Task<IEnumerable<OrderHistoryDto>> SearchOrderHistoryAsync(
        long orderId,
        int pageNumber,
        int pageSize,
        CancellationToken ct)
    {
        SearchOrderHistoryResponse response = await _client.SearchOrderHistoryAsync(
            new SearchOrderHistoryRequest
            {
                OrderId = orderId,
                PageNumber = pageNumber,
                PageSize = pageSize,
            },
            cancellationToken: ct);

        IEnumerable<OrderHistoryDto> items = response.Items.Select(item => new OrderHistoryDto(
            item.OrderHistoryItemId,
            item.OrderId,
            item.CreatedAt.ToDateTimeOffset(),
            item.OrderHistoryItemKind.MapItemKind(),
            item.MapPayload()));

        return items;
    }
}