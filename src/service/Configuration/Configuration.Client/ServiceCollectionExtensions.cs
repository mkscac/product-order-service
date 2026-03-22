using Configuration.Client.ConfigurationClients;
using Configuration.Client.ConfigurationClients.Manual;
using Configuration.Client.ConfigurationClients.Refit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;

namespace Configuration.Client;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConfigurationClientManual(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ConfigurationClientOptions>(configuration.GetSection("Configuration:Client"));

        services.AddHttpClient(name: "ConfigurationClient", configureClient: (provider, client) =>
        {
            string baseAddress =
                provider.GetRequiredService<IOptions<ConfigurationClientOptions>>().Value.BaseAddress;
            if (string.IsNullOrWhiteSpace(baseAddress))
                throw new InvalidOperationException("BaseAddress не настроен в ConfigurationClientOptions");

            client.BaseAddress = new Uri(baseAddress);
        });

        services.AddSingleton<IConfigurationClient, ManualConfigurationClient>();

        return services;
    }

    public static IServiceCollection AddConfigurationClientRefit(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ConfigurationClientOptions>(configuration.GetSection("Configuration:Client"));

        services
            .AddRefitClient<IConfigurationApiClient>()
            .ConfigureHttpClient((provider, client) =>
            {
                string baseAddress =
                    provider.GetRequiredService<IOptions<ConfigurationClientOptions>>().Value.BaseAddress;
                if (string.IsNullOrWhiteSpace(baseAddress))
                    throw new InvalidOperationException("BaseAddress не настроен в ConfigurationClientOptions");

                client.BaseAddress = new Uri(baseAddress);
            });

        services.AddSingleton<IConfigurationClient, RefitConfigurationClient>();

        return services;
    }
}