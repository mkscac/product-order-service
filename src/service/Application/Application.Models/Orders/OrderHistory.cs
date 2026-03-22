using Application.Models.Orders.OrderHistoryPayloads;

namespace Application.Models.Orders;

public record OrderHistory(
    long ItemId,
    long OrderId,
    DateTimeOffset CreatedAt,
    OrderHistoryItemKind ItemKind,
    OrderHistoryPayloadBase? Payload);