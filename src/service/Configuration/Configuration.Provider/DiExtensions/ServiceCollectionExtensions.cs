using Configuration.Provider.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Configuration.Provider.DiExtensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConfigurationUpdateService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ConfigurationUpdateOptions>(configuration.GetSection("Configuration:UpdateOptions"));

        services.AddSingleton<IConfigurationUpdateService, ConfigurationUpdateService>();

        services.AddHostedService<ConfigurationUpdateBackgroundService>();

        return services;
    }
}