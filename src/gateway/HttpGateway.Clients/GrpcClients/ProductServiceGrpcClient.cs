using HttpGateway.Models;
using Products.Contracts;

namespace HttpGateway.Clients.GrpcClients;

public class ProductServiceGrpcClient : IProductServiceGrpcClient
{
    private readonly MainProductService.MainProductServiceClient _client;

    public ProductServiceGrpcClient(MainProductService.MainProductServiceClient client)
    {
        _client = client;
    }

    public async Task<long> CreateProductAsync(string name, decimal price, CancellationToken ct)
    {
        CreateProductResponse response = await _client.CreateProductAsync(
            new CreateProductRequest
            {
                Name = name,
                Price = (long)price,
            },
            cancellationToken: ct);
        return response.ProductId;
    }
}