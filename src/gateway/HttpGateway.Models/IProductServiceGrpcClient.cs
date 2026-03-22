namespace HttpGateway.Models;

public interface IProductServiceGrpcClient
{
    Task<long> CreateProductAsync(string name, decimal price, CancellationToken ct);
}