using Application.Abstractions.Persistence;
using Application.Contracts;
using Application.Models.Orders;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Orders.Contracts;

namespace Presentation.Grpc.Services;

public class OrderServiceGrpc : MainOrderService.MainOrderServiceBase
{
    private readonly IOrderService _orderService;

    public OrderServiceGrpc(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public override async Task<CreateOrderResponse> CreateOrder(CreateOrderRequest request, ServerCallContext context)
    {
        return new CreateOrderResponse
        {
            OrderId = await _orderService.CreateAsync(request.CreatedBy, context.CancellationToken),
        };
    }

    public override async Task<AddProductInOrderResponse> AddProductInOrder(
        AddProductInOrderRequest request,
        ServerCallContext context)
    {
        return new AddProductInOrderResponse
        {
            OrderItemId = await _orderService.AddProductAsync(
                request.OrderId,
                request.ProductId,
                request.Quantity,
                context.CancellationToken),
        };
    }

    public override async Task<DeleteProductFromOrderResponse> DeleteProductFromOrder(
        DeleteProductFromOrderRequest request,
        ServerCallContext context)
    {
        await _orderService.DeleteProductAsync(request.OrderId, request.ProductId, context.CancellationToken);
        return new DeleteProductFromOrderResponse();
    }

    public override async Task<ExecutionOrderResponse> ExecutionOrder(
        ExecutionOrderRequest request,
        ServerCallContext context)
    {
        await _orderService.ExecutionOrderAsync(request.OrderId, context.CancellationToken);
        return new ExecutionOrderResponse();
    }

    public override async Task<CompletedOrderResponse> CompletedOrder(
        CompletedOrderRequest request,
        ServerCallContext context)
    {
        await _orderService.CompletedOrderAsync(request.OrderId, context.CancellationToken);
        return new CompletedOrderResponse();
    }

    public override async Task<CancelOrderResponse> CancelOrder(
        CancelOrderRequest request,
        ServerCallContext context)
    {
        await _orderService.CancelOrderAsync(request.OrderId, context.CancellationToken);
        return new CancelOrderResponse();
    }

    public override async Task<SearchOrderHistoryResponse> SearchOrderHistory(
        SearchOrderHistoryRequest request,
        ServerCallContext context)
    {
        Page<OrderHistory> page = await _orderService.SearchHistoryAsync(
            request.OrderId,
            new PaginationParams(request.PageNumber, request.PageSize),
            context.CancellationToken);

        var response = new SearchOrderHistoryResponse();

        foreach (OrderHistory item in page.Items)
        {
            var itemGrpc = new OrderHistoryItem
            {
                OrderHistoryItemId = item.ItemId,
                OrderId = item.OrderId,
                CreatedAt = Timestamp.FromDateTimeOffset(item.CreatedAt),
                OrderHistoryItemKind = item.ItemKind.MapItemKind(),
            };
            if (item.Payload is not null)
                itemGrpc.MapPayload(item.Payload);
            response.Items.Add(itemGrpc);
        }

        return response;
    }
}