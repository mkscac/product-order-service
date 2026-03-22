using HttpGateway.Models.Orders;
using HttpGateway.Models.Orders.History;
using HttpGateway.Models.Orders.History.Payloads;
using Orders.Contracts;

namespace HttpGateway.Clients;

public static class Mappings
{
    public static OrderHistoryPayloadBaseDto? MapPayload(this OrderHistoryItem item)
    {
        return item.PayloadCase switch
        {
            OrderHistoryItem.PayloadOneofCase.CreatedGrpc => new HistoryCreatedPayloadDto(
                item.CreatedGrpc.CreatedBy),
            OrderHistoryItem.PayloadOneofCase.AddedGrpc => new HistoryItemAddedPayloadDto(
                item.AddedGrpc.ProductId,
                item.AddedGrpc.Quantity),
            OrderHistoryItem.PayloadOneofCase.RemovedGrpc => new HistoryItemRemovedPayloadDto(
                item.RemovedGrpc.OrderItemId),
            OrderHistoryItem.PayloadOneofCase.StateChangedGrpc => new HistoryStateChangedPayloadDto(
                item.StateChangedGrpc.FromState.MapOrderState(),
                item.StateChangedGrpc.ToState.MapOrderState()),
            OrderHistoryItem.PayloadOneofCase.ProcessingStateChangedGrpc => new HistoryProcessingPayloadDto(
                item.ProcessingStateChangedGrpc.FromState,
                item.ProcessingStateChangedGrpc.ToState),
            OrderHistoryItem.PayloadOneofCase.None => null,
            _ => null,
        };
    }

    public static OrderHistoryItemKindDto MapItemKind(this OrderHistoryItemKindGrpc itemKindGrpc)
    {
        return itemKindGrpc switch
        {
            OrderHistoryItemKindGrpc.Created => OrderHistoryItemKindDto.Created,
            OrderHistoryItemKindGrpc.ItemAdded => OrderHistoryItemKindDto.ItemAdded,
            OrderHistoryItemKindGrpc.ItemRemoved => OrderHistoryItemKindDto.ItemRemoved,
            OrderHistoryItemKindGrpc.StateChanged => OrderHistoryItemKindDto.StateChanged,
            OrderHistoryItemKindGrpc.Unspecified => OrderHistoryItemKindDto.Unspecified,
            _ => OrderHistoryItemKindDto.Unspecified,
        };
    }

    private static OrderStateDto MapOrderState(this OrderStateGrpc stateGrpc)
    {
        return stateGrpc switch
        {
            OrderStateGrpc.Created => OrderStateDto.Created,
            OrderStateGrpc.Processing => OrderStateDto.Processing,
            OrderStateGrpc.Completed => OrderStateDto.Completed,
            OrderStateGrpc.Cancelled => OrderStateDto.Cancelled,
            OrderStateGrpc.Unspecified => OrderStateDto.Unspecified,
            _ => OrderStateDto.Unspecified,
        };
    }
}