using Application.Contracts;
using Grpc.Core;
using Products.Contracts;

namespace Presentation.Grpc.Services;

public class ProductServiceGrpc : MainProductService.MainProductServiceBase
{
    private readonly IProductService _productService;

    public ProductServiceGrpc(IProductService productService)
    {
        _productService = productService;
    }

    public override async Task<CreateProductResponse> CreateProduct(
        CreateProductRequest request,
        ServerCallContext context)
    {
        return new CreateProductResponse
        {
            ProductId = await _productService.CreateAsync(request.Name, request.Price, context.CancellationToken),
        };
    }
}