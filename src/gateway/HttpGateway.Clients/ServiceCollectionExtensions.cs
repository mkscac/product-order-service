using HttpGateway.Clients.GrpcClients;
using HttpGateway.Clients.GrpcClients.Options;
using HttpGateway.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HttpGateway.Clients;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationGrpcClients(
        this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.Configure<PresentationClientsOptions>(configuration.GetSection("GrpcClients:Presentation"));

        services
            .AddGrpcClient<Orders.Contracts.MainOrderService.MainOrderServiceClient>((sp, o) =>
            {
                PresentationClientsOptions options =
                    sp.GetRequiredService<IOptions<PresentationClientsOptions>>().Value;
                o.Address = new Uri(options.Url);
            });
        services
            .AddGrpcClient<Products.Contracts.MainProductService.MainProductServiceClient>((sp, o) =>
            {
                PresentationClientsOptions options =
                    sp.GetRequiredService<IOptions<PresentationClientsOptions>>().Value;
                o.Address = new Uri(options.Url);
            });

        services.AddScoped<IOrderServiceGrpcClient, OrderServiceGrpcClient>();
        services.AddScoped<IProductServiceGrpcClient, ProductServiceGrpcClient>();

        return services;
    }

    public static IServiceCollection AddProcessingGrpcClients(
        this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services
            .Configure<OrderProcessingClientOptions>(configuration.GetSection("GrpcClients:OrderProcessing"));

        services
            .AddGrpcClient
                <Orders.ProcessingService.Contracts.OrderService.OrderServiceClient>((sp, o) =>
                {
                    OrderProcessingClientOptions options =
                        sp.GetRequiredService<IOptions<OrderProcessingClientOptions>>().Value;
                    o.Address = new Uri(options.Url);
                });

        services.AddScoped<IOrderProcessingServiceGrpcClient, OrderProcessingServiceGrpcClient>();

        return services;
    }
}