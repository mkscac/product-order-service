using HttpGateway.Models;
using HttpGateway.Models.Orders.History;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HttpGateway.Controllers;

[ApiController]
[Route("gateway/orders")]
public class OrderController : ControllerBase
{
    private readonly IOrderServiceGrpcClient _client;

    public OrderController(IOrderServiceGrpcClient client)
    {
        _client = client;
    }

    [HttpPost]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<long>> CreateOrder([FromQuery][Required] string createdBy, CancellationToken ct)
    {
        long orderId = await _client.CreateOrderAsync(createdBy, ct);
        return Ok(orderId);
    }

    [HttpPost("{orderId:long}/product/{productId:long}")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(statusCode: StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<long>> AddProductInOrder(
        [FromRoute][Required] long orderId,
        [FromRoute][Required] long productId,
        [FromQuery][Required] int quantity,
        CancellationToken ct)
    {
        long orderItemId = await _client.AddProductInOrderAsync(orderId, productId, quantity, ct);
        return Ok(orderItemId);
    }

    [HttpDelete("{orderId:long}/product/{productId:long}")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
    [ProducesResponseType(statusCode: StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteProductFromOrder(
        [FromRoute][Required] long orderId,
        [FromRoute][Required] long productId,
        CancellationToken ct)
    {
        await _client.DeleteProductFromOrderAsync(orderId, productId, ct);
        return Ok();
    }

    [HttpPatch("{orderId:long}/status/processing")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
    [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ExecutionOrder([FromRoute][Required] long orderId, CancellationToken ct)
    {
        await _client.ExecutionOrderAsync(orderId, ct);
        return Ok();
    }

    [HttpPatch("{orderId:long}/status/cancelled")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
    [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CancelOrder([FromRoute][Required] long orderId, CancellationToken ct)
    {
        await _client.CancelOrderAsync(orderId, ct);
        return Ok();
    }

    [HttpGet("{orderId:long}/history")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
    [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<OrderHistoryDto>>> GetOrderHistory(
        [FromRoute][Required] long orderId,
        [FromQuery][Required] int pageNumber,
        [FromQuery][Required] int pageSize,
        CancellationToken ct)
    {
        IEnumerable<OrderHistoryDto> items =
            await _client.SearchOrderHistoryAsync(orderId, pageNumber, pageSize, ct);
        return Ok(items);
    }
}