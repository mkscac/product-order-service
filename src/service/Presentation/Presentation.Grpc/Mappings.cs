using Application.Models.Orders;
using Application.Models.Orders.OrderHistoryPayloads;
using Orders.Contracts;

namespace Presentation.Grpc;

public static class Mappings
{
    public static OrderHistoryItemKindGrpc MapItemKind(this OrderHistoryItemKind kind)
    {
        return kind switch
        {
            OrderHistoryItemKind.Created => OrderHistoryItemKindGrpc.Created,
            OrderHistoryItemKind.ItemAdded => OrderHistoryItemKindGrpc.ItemAdded,
            OrderHistoryItemKind.ItemRemoved => OrderHistoryItemKindGrpc.ItemRemoved,
            OrderHistoryItemKind.StateChanged => OrderHistoryItemKindGrpc.StateChanged,
            _ => OrderHistoryItemKindGrpc.Unspecified,
        };
    }

    public static void MapPayload(this OrderHistoryItem item, OrderHistoryPayloadBase payload)
    {
        switch (payload)
        {
            case HistoryCreatedPayload created:
                item.CreatedGrpc = new HistoryCreatedPayloadGrpc { CreatedBy = created.CreatedBy };
                break;
            case HistoryItemAddedPayload added:
                item.AddedGrpc = new HistoryItemAddedPayloadGrpc { ProductId = added.ProductId, Quantity = added.Quantity };
                break;
            case HistoryItemRemovedPayload removed:
                item.RemovedGrpc = new HistoryItemRemovedPayloadGrpc { OrderItemId = removed.OrderItemId };
                break;
            case HistoryStateChangedPayload stateChanged:
                item.StateChangedGrpc = new HistoryStateChangedPayloadGrpc
                {
                    FromState = stateChanged.FromState.MapEnumOrderState(),
                    ToState = stateChanged.ToState.MapEnumOrderState(),
                };
                break;
            case HistoryProcessingStateChangedPayload presentationStateChanged:
                item.ProcessingStateChangedGrpc = new HistoryProcessingStateChangedPayloadGrpc
                {
                    FromState = presentationStateChanged.FromState,
                    ToState = presentationStateChanged.ToState,
                };
                break;
            default:
                break;
        }
    }

    private static OrderStateGrpc MapEnumOrderState(this OrderState state)
    {
        return state switch
        {
            OrderState.Created => OrderStateGrpc.Created,
            OrderState.Processing => OrderStateGrpc.Processing,
            OrderState.Completed => OrderStateGrpc.Completed,
            OrderState.Cancelled => OrderStateGrpc.Cancelled,
            _ => OrderStateGrpc.Unspecified,
        };
    }
}