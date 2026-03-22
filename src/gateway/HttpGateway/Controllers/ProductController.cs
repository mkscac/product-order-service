using HttpGateway.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HttpGateway.Controllers;

[ApiController]
[Route("gateway/products")]
public class ProductController : ControllerBase
{
    private readonly IProductServiceGrpcClient _client;

    public ProductController(IProductServiceGrpcClient client)
    {
        _client = client;
    }

    [HttpPost]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<long>> CreateProduct(
        [FromQuery][Required] string name,
        [FromQuery][Required] decimal price,
        CancellationToken ct)
    {
        long productId = await _client.CreateProductAsync(name, price, ct);
        return Ok(productId);
    }
}