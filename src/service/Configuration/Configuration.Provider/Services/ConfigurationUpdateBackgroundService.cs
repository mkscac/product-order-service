using Microsoft.Extensions.Hosting;

namespace Configuration.Provider.Services;

public class ConfigurationUpdateBackgroundService : BackgroundService
{
    private readonly IConfigurationUpdateService _service;

    public ConfigurationUpdateBackgroundService(IConfigurationUpdateService service)
    {
        _service = service;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _service.RunPeriodicUpdateAsync(stoppingToken);
    }
}