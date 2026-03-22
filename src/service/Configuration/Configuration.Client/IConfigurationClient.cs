using Configuration.Models;

namespace Configuration.Client;

public interface IConfigurationClient
{
    IAsyncEnumerable<ConfigurationItem> GetAllConfigurationsAsync(CancellationToken ct);
}