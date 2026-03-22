namespace HttpGateway.Models.Orders.History.Payloads;

public sealed record HistoryCreatedPayloadDto(string CreatedBy) : OrderHistoryPayloadBaseDto;