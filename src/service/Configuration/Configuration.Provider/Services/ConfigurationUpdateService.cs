using Configuration.Client;
using Configuration.Models;
using Configuration.Provider.Providers;
using Microsoft.Extensions.Options;

namespace Configuration.Provider.Services;

public class ConfigurationUpdateService : IConfigurationUpdateService
{
    private readonly IConfigurationClient _client;
    private readonly CustomConfigurationProvider _provider;
    private readonly IOptions<ConfigurationUpdateOptions> _options;

    public ConfigurationUpdateService(
        IConfigurationClient client,
        CustomConfigurationProvider provider,
        IOptions<ConfigurationUpdateOptions> options)
    {
        _client = client;
        _provider = provider;
        _options = options;
    }

    public async Task<bool> UpdateOnceAsync(CancellationToken ct)
    {
        List<ConfigurationItem> items = await _client.GetAllConfigurationsAsync(ct).ToListAsync(ct);
        return _provider.TryApplyConfigurationItems(items);
    }

    public async Task RunPeriodicUpdateAsync(CancellationToken ct)
    {
        using var timer = new PeriodicTimer(period: TimeSpan.FromSeconds(_options.Value.UpdateIntervalSeconds));
        await UpdateOnceAsync(ct);

        while (await timer.WaitForNextTickAsync(ct))
        {
            await UpdateOnceAsync(ct);
        }
    }
}