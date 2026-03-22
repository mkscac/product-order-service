using Configuration.Models;
using Refit;

namespace Configuration.Client.ConfigurationClients.Refit;

public interface IConfigurationApiClient
{
    [Get("/configurations")]
    Task<ConfigurationPage> GetAsync(int pageSize, string? pageToken, CancellationToken cancellationToken);
}