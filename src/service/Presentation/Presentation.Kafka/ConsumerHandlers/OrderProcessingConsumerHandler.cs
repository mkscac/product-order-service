using Application.Abstractions.Persistence.Repositories;
using Application.Contracts;
using Application.Contracts.OrderProcessing;
using Application.Models.Orders;
using Application.Models.Orders.OrderHistoryPayloads;
using Kafka.Consumer;
using Orders.Kafka.Contracts;

namespace Presentation.Kafka.ConsumerHandlers;

public class OrderProcessingConsumerHandler : IKafkaConsumerHandler<OrderProcessingKey, OrderProcessingValue>
{
    private readonly IOrderService _orderService;
    private readonly IOrderHistoryRepository _historyRepository;

    public OrderProcessingConsumerHandler(IOrderService orderService, IOrderHistoryRepository historyRepository)
    {
        _orderService = orderService;
        _historyRepository = historyRepository;
    }

    public async Task HandleBatchAsync(
        IEnumerable<IKafkaConsumerMessage<OrderProcessingKey, OrderProcessingValue>> messages,
        CancellationToken ct)
    {
        foreach (IKafkaConsumerMessage<OrderProcessingKey, OrderProcessingValue> message in messages)
        {
            OrderProcessingValue value = message.Value;

            switch (value.EventCase)
            {
                case OrderProcessingValue.EventOneofCase.ApprovalReceived:
                    await HandleApprovalReceived(value.ApprovalReceived, ct);
                    break;

                case OrderProcessingValue.EventOneofCase.PackingStarted:
                    await HandlePackingStarted(value.PackingStarted, ct);
                    break;

                case OrderProcessingValue.EventOneofCase.PackingFinished:
                    await HandlePackingFinished(value.PackingFinished, ct);
                    break;

                case OrderProcessingValue.EventOneofCase.DeliveryStarted:
                    await HandleDeliveryStarted(value.DeliveryStarted, ct);
                    break;

                case OrderProcessingValue.EventOneofCase.DeliveryFinished:
                    await HandleDeliveryFinished(value.DeliveryFinished, ct);
                    break;

                case OrderProcessingValue.EventOneofCase.None:
                    continue;
                default:
                    throw new InvalidOperationException($"Ошибка. Неизвестный тип события: {value.EventCase}");
            }
        }
    }

    private async Task HandleApprovalReceived(
        OrderProcessingValue.Types.OrderApprovalReceived orderEvent,
        CancellationToken ct)
    {
        if (!orderEvent.IsApproved)
        {
            await _orderService.CancelOrderAsync(orderEvent.OrderId, ct);
            return;
        }

        await _historyRepository.AddNoteAsync(
            orderEvent.OrderId,
            OrderHistoryItemKind.StateChanged,
            new HistoryProcessingStateChangedPayload(
                nameof(OrderState.Processing),
                nameof(OrderProcessingState.Approved)),
            ct);
    }

    private async Task HandlePackingStarted(
        OrderProcessingValue.Types.OrderPackingStarted orderEvent,
        CancellationToken ct)
    {
        await _historyRepository.AddNoteAsync(
            orderEvent.OrderId,
            OrderHistoryItemKind.StateChanged,
            new HistoryProcessingStateChangedPayload(
                nameof(OrderProcessingState.Approved),
                nameof(OrderProcessingState.Packing)),
            ct);
    }

    private async Task HandlePackingFinished(
        OrderProcessingValue.Types.OrderPackingFinished orderEvent,
        CancellationToken ct)
    {
        if (!orderEvent.IsFinishedSuccessfully)
        {
            await _orderService.CancelOrderAsync(orderEvent.OrderId, ct);
            return;
        }

        await _historyRepository.AddNoteAsync(
            orderEvent.OrderId,
            OrderHistoryItemKind.StateChanged,
            new HistoryProcessingStateChangedPayload(
                nameof(OrderProcessingState.Packing),
                nameof(OrderProcessingState.Packed)),
            ct);
    }

    private async Task HandleDeliveryStarted(
        OrderProcessingValue.Types.OrderDeliveryStarted orderEvent,
        CancellationToken ct)
    {
        await _historyRepository.AddNoteAsync(
            orderEvent.OrderId,
            OrderHistoryItemKind.StateChanged,
            new HistoryProcessingStateChangedPayload(
                nameof(OrderProcessingState.Packed),
                nameof(OrderProcessingState.InDelivery)),
            ct);
    }

    private async Task HandleDeliveryFinished(
        OrderProcessingValue.Types.OrderDeliveryFinished orderEvent,
        CancellationToken ct)
    {
        if (!orderEvent.IsFinishedSuccessfully)
        {
            await _orderService.CancelOrderAsync(orderEvent.OrderId, ct);
            return;
        }

        await _historyRepository.AddNoteAsync(
            orderEvent.OrderId,
            OrderHistoryItemKind.StateChanged,
            new HistoryProcessingStateChangedPayload(
                nameof(OrderProcessingState.InDelivery),
                nameof(OrderProcessingState.Delivered)),
            ct);

        await _orderService.CompletedOrderAsync(orderEvent.OrderId, ct);
    }
}