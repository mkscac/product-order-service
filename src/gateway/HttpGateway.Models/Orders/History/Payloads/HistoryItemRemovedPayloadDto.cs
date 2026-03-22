namespace HttpGateway.Models.Orders.History.Payloads;

public sealed record HistoryItemRemovedPayloadDto(long OrderItemId) : OrderHistoryPayloadBaseDto;