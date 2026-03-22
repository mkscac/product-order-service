using HttpGateway.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HttpGateway.Controllers;

[ApiController]
[Route("gateway/processing/orders/{orderId:long}")]
public class OrderProcessingController : ControllerBase
{
    private readonly IOrderProcessingServiceGrpcClient _orderGrpcClient;

    public OrderProcessingController(IOrderProcessingServiceGrpcClient orderGrpcClient)
    {
        _orderGrpcClient = orderGrpcClient;
    }

    [HttpPatch("approved")]
    public async Task<ActionResult> ApproveOrder(
        [FromRoute][Required] long orderId,
        [FromQuery][Required] bool isApproved,
        [FromQuery][Required] string approvedBy,
        [FromQuery] string? failureReason,
        CancellationToken ct)
    {
        await _orderGrpcClient.ApproveOrderAsync(orderId, isApproved, approvedBy, failureReason, ct);
        return Ok();
    }

    [HttpPatch("packing/start")]
    public async Task<ActionResult> StartOrderPacking(
        [FromRoute][Required] long orderId,
        [FromQuery][Required] string packingBy,
        CancellationToken ct)
    {
        await _orderGrpcClient.StartOrderPackingAsync(orderId, packingBy, ct);
        return Ok();
    }

    [HttpPatch("packing/finish")]
    public async Task<ActionResult> FinishOrderPacking(
        [FromRoute][Required] long orderId,
        [FromQuery][Required] bool isSuccessful,
        [FromQuery] string? failureReason,
        CancellationToken ct)
    {
        await _orderGrpcClient.FinishOrderPackingAsync(orderId, isSuccessful, failureReason, ct);
        return Ok();
    }

    [HttpPatch("delivery/start")]
    public async Task<ActionResult> StartOrderDelivery(
        [FromRoute][Required] long orderId,
        [FromQuery][Required] string deliveryBy,
        CancellationToken ct)
    {
        await _orderGrpcClient.StartOrderDeliveryAsync(orderId, deliveryBy, ct);
        return Ok();
    }

    [HttpPatch("delivery/finish")]
    public async Task<ActionResult> FinishOrderDelivery(
        [FromRoute][Required] long orderId,
        [FromQuery][Required] bool isSuccessful,
        [FromQuery] string? failureReason,
        CancellationToken ct)
    {
        await _orderGrpcClient.FinishOrderDeliveryAsync(orderId, isSuccessful, failureReason, ct);
        return Ok();
    }
}