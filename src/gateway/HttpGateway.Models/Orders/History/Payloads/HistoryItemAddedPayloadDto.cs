namespace HttpGateway.Models.Orders.History.Payloads;

public sealed record HistoryItemAddedPayloadDto(long ProductId, int Quantity) : OrderHistoryPayloadBaseDto;