using Application.Abstractions.Persistence.Repositories;
using Application.Models.Orders;
using FluentMigrator.Runner;
using Infrastructure.Persistence.Options;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructurePersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(
            configuration.GetSection("Infrastructure:Persistence:DatabaseOptions"));

        services.AddSingleton(serviceProvider =>
        {
            string connectionString =
                serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value.ConvertToConnectionString();
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
            dataSourceBuilder.MapEnum<OrderState>("order_state");
            dataSourceBuilder.MapEnum<OrderHistoryItemKind>("order_history_item_kind");
            return dataSourceBuilder.Build();
        });

        services.AddScoped<IProductRepository, NpgsqlProductRepository>();
        services.AddScoped<IOrderRepository, NpgsqlOrderRepository>();
        services.AddScoped<IOrderItemRepository, NpgsqlOrderItemRepository>();
        services.AddScoped<IOrderHistoryRepository, NpgsqlOrderHistoryRepository>();

        return services;
    }

    public static IServiceCollection AddMigrations(this IServiceCollection services)
    {
        services
            .AddFluentMigratorCore()
            .ConfigureRunner(runner => runner
                .AddPostgres()
                .WithGlobalConnectionString(serviceProvider =>
                    serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value.ConvertToConnectionString())
                .WithMigrationsIn(typeof(IMigrationAssemblyMarker).Assembly));
        return services;
    }

    public static async Task RunMigrations(this IServiceProvider provider)
    {
        await using AsyncServiceScope scope = provider.CreateAsyncScope();
        IMigrationRunner runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }

    public static async Task StopMigrationByVersion(this IServiceProvider provider, long version)
    {
        await using AsyncServiceScope scope = provider.CreateAsyncScope();
        IMigrationRunner runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateDown(version);
    }
}