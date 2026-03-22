using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Presentation.Grpc.Services;

namespace Presentation.Grpc;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationGrpc(this IServiceCollection services)
    {
        services.AddGrpc(grpc => grpc.Interceptors.Add<GrpcServerInterceptor>());
        return services;
    }

    public static WebApplication MapPresentationGrpc(this WebApplication app)
    {
        app.MapGrpcService<OrderServiceGrpc>();
        app.MapGrpcService<ProductServiceGrpc>();

        return app;
    }
}