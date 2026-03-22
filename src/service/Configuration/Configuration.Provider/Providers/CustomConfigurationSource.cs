using Microsoft.Extensions.Configuration;

namespace Configuration.Provider.Providers;

public class CustomConfigurationSource : IConfigurationSource
{
    private readonly CustomConfigurationProvider _provider;

    public CustomConfigurationSource(CustomConfigurationProvider provider)
    {
        _provider = provider;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return _provider;
    }
}