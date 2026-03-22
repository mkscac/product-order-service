namespace Application.Contracts;

public interface IProductService
{
    Task<long> CreateAsync(string name, decimal price, CancellationToken ct);
}