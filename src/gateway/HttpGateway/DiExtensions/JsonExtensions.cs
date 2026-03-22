using System.Text.Json.Serialization;

namespace HttpGateway.DiExtensions;

public static class JsonExtensions
{
    public static IMvcBuilder AddCustomJsonOptions(this IMvcBuilder builder)
    {
        builder
            .AddJsonOptions(o =>
                o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false)));
        return builder;
    }
}