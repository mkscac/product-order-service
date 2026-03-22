using Application.Abstractions.Persistence;
using Application.Abstractions.Persistence.Queries;
using Application.Abstractions.Persistence.Repositories;
using Application.Contracts;
using Application.Exceptions;
using Application.Models.Orders;
using Application.Models.Orders.OrderHistoryPayloads;
using Presentation.Kafka;
using System.Transactions;

namespace Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IOrderHistoryRepository _historyRepository;
    private readonly IOrderCreationProducer _producer;

    public OrderService(
        IOrderRepository orderRepository,
        IOrderItemRepository orderItemRepository,
        IOrderHistoryRepository orderHistoryRepository,
        IOrderCreationProducer producer)
    {
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _historyRepository = orderHistoryRepository;
        _producer = producer;
    }

    public async Task<long> CreateAsync(string createdBy, CancellationToken ct)
    {
        using var transactionScope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        long orderId = await _orderRepository.CreateAsync(createdBy, ct);
        await _historyRepository.AddNoteAsync(
            orderId,
            OrderHistoryItemKind.Created,
            new HistoryCreatedPayload(createdBy),
            ct);
        await _producer.ProduceOrderCreatedAsync(orderId, ct);

        transactionScope.Complete();

        return orderId;
    }

    public async Task<long> AddProductAsync(
        long orderId,
        long productId,
        int quantity,
        CancellationToken ct)
    {
        OrderState currentState = await GetCurrentOrderStateAsync(orderId, ct);
        if (currentState != OrderState.Created)
            throw new NotCorrectOrderStateException("Добавление");

        using var transactionScope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        long orderItemId = await _orderItemRepository.AddAsync(orderId, productId, quantity, ct);
        await _historyRepository.AddNoteAsync(
            orderId,
            OrderHistoryItemKind.ItemAdded,
            new HistoryItemAddedPayload(productId, quantity),
            ct);

        transactionScope.Complete();

        return orderItemId;
    }

    public async Task DeleteProductAsync(long orderId, long productId, CancellationToken ct)
    {
        OrderState currentState = await GetCurrentOrderStateAsync(orderId, ct);
        if (currentState != OrderState.Created)
            throw new NotCorrectOrderStateException("Удаление");

        using var transactionScope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        await _orderItemRepository.DeleteAsync(orderId, productId, ct);
        await _historyRepository.AddNoteAsync(
            orderId,
            OrderHistoryItemKind.ItemRemoved,
            new HistoryItemRemovedPayload(productId),
            ct);

        transactionScope.Complete();
    }

    public async Task ExecutionOrderAsync(long orderId, CancellationToken ct)
    {
        OrderState fromState = await GetCurrentOrderStateAsync(orderId, ct);
        if (fromState == OrderState.Processing)
            throw new StateAlreadyAppliedException(orderId, fromState);

        using var transactionScope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        await _orderRepository.ChangeStateAsync(orderId, OrderState.Processing, ct);
        await _historyRepository.AddNoteAsync(
            orderId,
            OrderHistoryItemKind.StateChanged,
            new HistoryStateChangedPayload(fromState, OrderState.Processing),
            ct);
        await _producer.ProduceOrderProcessingStartedAsync(orderId, ct);

        transactionScope.Complete();
    }

    public async Task CompletedOrderAsync(long orderId, CancellationToken ct)
    {
        OrderState fromState = await GetCurrentOrderStateAsync(orderId, ct);
        if (fromState == OrderState.Completed)
            throw new StateAlreadyAppliedException(orderId, fromState);

        using var transactionScope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        await _orderRepository.ChangeStateAsync(orderId, OrderState.Completed, ct);
        await _historyRepository.AddNoteAsync(
            orderId,
            OrderHistoryItemKind.StateChanged,
            new HistoryStateChangedPayload(fromState, OrderState.Completed),
            ct);

        transactionScope.Complete();
    }

    public async Task CancelOrderAsync(long orderId, CancellationToken ct)
    {
        OrderState fromState = await GetCurrentOrderStateAsync(orderId, ct);
        if (fromState == OrderState.Cancelled)
            throw new StateAlreadyAppliedException(orderId, fromState);

        using var transactionScope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        await _orderRepository.ChangeStateAsync(orderId, OrderState.Cancelled, ct);
        await _historyRepository.AddNoteAsync(
            orderId,
            OrderHistoryItemKind.StateChanged,
            new HistoryStateChangedPayload(fromState, OrderState.Cancelled),
            ct);

        transactionScope.Complete();
    }

    public async Task<Page<OrderHistory>> SearchHistoryAsync(
        long orderId,
        PaginationParams paginationParams,
        CancellationToken ct)
    {
        Page<OrderHistory> pageOrderHistory = await _historyRepository.SearchAsync(
            new OrderHistoryQuery(new List<long> { orderId }, null),
            new PaginationParams(paginationParams.PageNumber, paginationParams.PageSize),
            ct);
        return pageOrderHistory.Items.Count == 0 ?
            throw new NotFoundException(nameof(Order), orderId) : pageOrderHistory;
    }

    private async Task<OrderState> GetCurrentOrderStateAsync(long orderId, CancellationToken ct)
    {
        Page<Order> pageOrders = await _orderRepository.SearchAsync(
            new OrderQuery(new List<long> { orderId }, null, null),
            new PaginationParams(1, 100),
            ct);

        if (pageOrders.Items.Count == 0)
            throw new NotFoundException(nameof(Order), orderId);

        return pageOrders.Items[0].OrderState;
    }
}