namespace Application.Models.Orders.OrderHistoryPayloads;

public record HistoryItemRemovedPayload(long OrderItemId) : OrderHistoryPayloadBase;