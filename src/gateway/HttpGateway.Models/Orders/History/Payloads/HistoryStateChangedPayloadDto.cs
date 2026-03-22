namespace HttpGateway.Models.Orders.History.Payloads;

public sealed record HistoryStateChangedPayloadDto(OrderStateDto FromState, OrderStateDto ToState) : OrderHistoryPayloadBaseDto;