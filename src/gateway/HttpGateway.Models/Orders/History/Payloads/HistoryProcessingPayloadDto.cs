namespace HttpGateway.Models.Orders.History.Payloads;

public record HistoryProcessingPayloadDto(string FromState, string ToState) : OrderHistoryPayloadBaseDto;