using Application.Abstractions.Persistence.Queries;
using Application.Models.Products;

namespace Application.Abstractions.Persistence.Repositories;

public interface IProductRepository
{
    Task<long> CreateAsync(string name, decimal price, CancellationToken ct);

    Task<Page<Product>> SearchAsync(
        ProductQuery query,
        PaginationParams parameters,
        CancellationToken ct);
}