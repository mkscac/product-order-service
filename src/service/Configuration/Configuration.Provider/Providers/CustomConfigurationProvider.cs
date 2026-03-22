using Configuration.Models;
using Microsoft.Extensions.Configuration;

namespace Configuration.Provider.Providers;

public class CustomConfigurationProvider : ConfigurationProvider
{
    public bool TryApplyConfigurationItems(IEnumerable<ConfigurationItem> items)
    {
        var newConfiguration = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        foreach (ConfigurationItem item in items)
            newConfiguration[item.Key] = item.Value;

        if (IsDictionaryEquals(current: Data, other: newConfiguration))
            return false;

        Data = newConfiguration;
        OnReload();
        return true;
    }

    private static bool IsDictionaryEquals(IDictionary<string, string?> current, Dictionary<string, string?> other)
    {
        if (current.Count != other.Count)
            return false;

        foreach (KeyValuePair<string, string?> curr in current)
        {
            if (!other.TryGetValue(curr.Key, out string? otherValue))
                return false;
            if (!string.Equals(curr.Value, otherValue, StringComparison.Ordinal))
                return false;
        }

        return true;
    }
}