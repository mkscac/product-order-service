using HttpGateway.Models;
using Orders.ProcessingService.Contracts;

namespace HttpGateway.Clients.GrpcClients;

public class OrderProcessingServiceGrpcClient : IOrderProcessingServiceGrpcClient
{
    private readonly OrderService.OrderServiceClient _client;

    public OrderProcessingServiceGrpcClient(OrderService.OrderServiceClient client)
    {
        _client = client;
    }

    public async Task ApproveOrderAsync(
        long orderId,
        bool isApproved,
        string approvedBy,
        string? failureReason,
        CancellationToken ct)
    {
        var request = new ApproveOrderRequest
        {
            OrderId = orderId,
            IsApproved = isApproved,
            ApprovedBy = approvedBy,
            FailureReason = failureReason,
        };

        await _client.ApproveOrderAsync(request, cancellationToken: ct);
    }

    public async Task StartOrderPackingAsync(long orderId, string packingBy, CancellationToken ct)
    {
        var request = new StartOrderPackingRequest
        {
            OrderId = orderId,
            PackingBy = packingBy,
        };

        await _client.StartOrderPackingAsync(request, cancellationToken: ct);
    }

    public async Task FinishOrderPackingAsync(
        long orderId,
        bool isSuccessful,
        string? failureReason,
        CancellationToken ct)
    {
        var request = new FinishOrderPackingRequest
        {
            OrderId = orderId,
            IsSuccessful = isSuccessful,
            FailureReason = failureReason,
        };

        await _client.FinishOrderPackingAsync(request, cancellationToken: ct);
    }

    public async Task StartOrderDeliveryAsync(long orderId, string deliveryBy, CancellationToken ct)
    {
        var request = new StartOrderDeliveryRequest
        {
            OrderId = orderId,
            DeliveredBy = deliveryBy,
        };

        await _client.StartOrderDeliveryAsync(request, cancellationToken: ct);
    }

    public async Task FinishOrderDeliveryAsync(
        long orderId,
        bool isSuccessful,
        string? failureReason,
        CancellationToken ct)
    {
        var request = new FinishOrderDeliveryRequest
        {
            OrderId = orderId,
            IsSuccessful = isSuccessful,
            FailureReason = failureReason,
        };

        await _client.FinishOrderDeliveryAsync(request, cancellationToken: ct);
    }
}