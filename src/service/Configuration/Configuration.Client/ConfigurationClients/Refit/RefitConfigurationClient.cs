using Configuration.Models;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

namespace Configuration.Client.ConfigurationClients.Refit;

public class RefitConfigurationClient : IConfigurationClient
{
    private readonly IConfigurationApiClient _configurationApiClient;
    private readonly IOptionsMonitor<ConfigurationClientOptions> _optionsMonitor;

    public RefitConfigurationClient(
        IConfigurationApiClient configurationApiClient,
        IOptionsMonitor<ConfigurationClientOptions> optionsMonitor)
    {
        _configurationApiClient = configurationApiClient;
        _optionsMonitor = optionsMonitor;
    }

    public async IAsyncEnumerable<ConfigurationItem> GetAllConfigurationsAsync(
        [EnumeratorCancellation] CancellationToken ct)
    {
        string? pageToken = null;

        while (true)
        {
            ConfigurationPage page = await _configurationApiClient.GetAsync(
                _optionsMonitor.CurrentValue.PageSize, pageToken, ct);

            if (page.Items is { Count: > 0 })
            {
                foreach (ConfigurationItem item in page.Items)
                    yield return item;
            }

            pageToken = page.PageToken;

            if (string.IsNullOrWhiteSpace(page.PageToken))
                yield break;
        }
    }
}