using Configuration.Provider.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Configuration.Provider.DiExtensions;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddCustomConfigurationProvider(
        this IConfigurationBuilder builder,
        IServiceCollection services)
    {
        var provider = new CustomConfigurationProvider();
        builder.Add(new CustomConfigurationSource(provider));

        services.AddSingleton(provider);

        return builder;
    }
}