namespace Application.Models.Orders.OrderHistoryPayloads;

public sealed record HistoryCreatedPayload(string CreatedBy) : OrderHistoryPayloadBase;