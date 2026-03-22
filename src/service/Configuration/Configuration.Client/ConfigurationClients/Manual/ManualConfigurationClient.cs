using Configuration.Models;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Configuration.Client.ConfigurationClients.Manual;

public class ManualConfigurationClient : IConfigurationClient
{
    private const string ClientName = "ConfigurationClient";
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptionsMonitor<ConfigurationClientOptions> _optionsMonitor;

    public ManualConfigurationClient(
        IHttpClientFactory httpClientFactory,
        IOptionsMonitor<ConfigurationClientOptions> optionsMonitor)
    {
        _httpClientFactory = httpClientFactory;
        _optionsMonitor = optionsMonitor;
    }

    public async IAsyncEnumerable<ConfigurationItem> GetAllConfigurationsAsync(
        [EnumeratorCancellation] CancellationToken ct)
    {
        HttpClient httpClient = _httpClientFactory.CreateClient(ClientName);
        string? pageToken = null;

        while (true)
        {
            string url = pageToken is null ?
                $"configurations?pageSize={_optionsMonitor.CurrentValue.PageSize}" :
                $"configurations?pageSize={_optionsMonitor.CurrentValue.PageSize}&pageToken={Uri.EscapeDataString(pageToken)}";

            string json = await httpClient.GetStringAsync(url, ct);
            ConfigurationPage? page = JsonSerializer.Deserialize<ConfigurationPage>(json);

            if (page?.Items is { Count: > 0 })
            {
                foreach (ConfigurationItem item in page.Items)
                    yield return item;
            }

            pageToken = page?.PageToken;

            if (string.IsNullOrWhiteSpace(page?.PageToken))
                yield break;
        }
    }
}