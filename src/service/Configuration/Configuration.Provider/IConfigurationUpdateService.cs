namespace Configuration.Provider;

public interface IConfigurationUpdateService
{
    Task<bool> UpdateOnceAsync(CancellationToken ct);

    Task RunPeriodicUpdateAsync(CancellationToken ct);
}