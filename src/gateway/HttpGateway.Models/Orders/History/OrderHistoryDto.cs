using HttpGateway.Models.Orders.History.Payloads;

namespace HttpGateway.Models.Orders.History;

public sealed record OrderHistoryDto(
    long ItemId,
    long OrderId,
    DateTimeOffset CreatedAt,
    OrderHistoryItemKindDto ItemKind,
    OrderHistoryPayloadBaseDto? Payload);