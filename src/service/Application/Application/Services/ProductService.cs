using Application.Abstractions.Persistence.Repositories;
using Application.Contracts;

namespace Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<long> CreateAsync(string name, decimal price, CancellationToken ct)
    {
        return await _productRepository.CreateAsync(name, price, ct);
    }
}