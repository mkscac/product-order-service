namespace Application.Models.Orders.OrderHistoryPayloads;

public record HistoryProcessingStateChangedPayload(string FromState, string ToState) : OrderHistoryPayloadBase;