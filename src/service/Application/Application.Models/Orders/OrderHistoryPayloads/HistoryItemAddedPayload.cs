namespace Application.Models.Orders.OrderHistoryPayloads;

public sealed record HistoryItemAddedPayload(long ProductId, int Quantity) : OrderHistoryPayloadBase;