namespace Application.Models.Orders.OrderHistoryPayloads;

public sealed record HistoryStateChangedPayload(OrderState FromState, OrderState ToState) : OrderHistoryPayloadBase;