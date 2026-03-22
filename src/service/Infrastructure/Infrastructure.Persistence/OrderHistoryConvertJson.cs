using Application.Models.Orders.OrderHistoryPayloads;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Persistence;

public static class OrderHistoryConvertJson
{
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
    {
        Converters =
        {
            new JsonStringEnumConverter(allowIntegerValues: false),
        },
    };

    public static string Serialize(OrderHistoryPayloadBase payload)
    {
        return JsonSerializer.Serialize(payload, Options);
    }

    public static OrderHistoryPayloadBase? Deserialize(string payload)
    {
        return JsonSerializer.Deserialize<OrderHistoryPayloadBase>(payload, Options);
    }
}