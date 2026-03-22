namespace HttpGateway.Models.Orders.History;

public enum OrderHistoryItemKindDto
{
    Unspecified,
    Created,
    ItemAdded,
    ItemRemoved,
    StateChanged,
}