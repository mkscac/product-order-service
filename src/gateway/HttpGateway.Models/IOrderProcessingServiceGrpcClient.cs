namespace HttpGateway.Models;

public interface IOrderProcessingServiceGrpcClient
{
    Task ApproveOrderAsync(
        long orderId,
        bool isApproved,
        string approvedBy,
        string? failureReason,
        CancellationToken ct);

    Task StartOrderPackingAsync(long orderId, string packingBy, CancellationToken ct);

    Task FinishOrderPackingAsync(
        long orderId,
        bool isSuccessful,
        string? failureReason,
        CancellationToken ct);

    Task StartOrderDeliveryAsync(long orderId, string deliveryBy, CancellationToken ct);

    Task FinishOrderDeliveryAsync(
        long orderId,
        bool isSuccessful,
        string? failureReason,
        CancellationToken ct);
}