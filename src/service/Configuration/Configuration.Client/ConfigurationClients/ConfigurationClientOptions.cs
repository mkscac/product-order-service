namespace Configuration.Client.ConfigurationClients;

public class ConfigurationClientOptions
{
    public string BaseAddress { get; set; } = string.Empty;

    public int PageSize { get; set; } = 1;
}