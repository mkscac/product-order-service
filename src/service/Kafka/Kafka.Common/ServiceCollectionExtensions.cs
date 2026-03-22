using Confluent.Kafka;
using Google.Protobuf;
using Kafka.Common.Serializers;
using Microsoft.Extensions.DependencyInjection;

namespace Kafka.Common;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProtobufSerializerDeserializer<T>(
        this IServiceCollection services,
        string name) where T : IMessage<T>, new()
    {
        services
            .AddKeyedSingleton<ISerializer<T>>(name, (_, _) => new ProtobufSerializer<T>())
            .AddKeyedSingleton<IDeserializer<T>>(name, (_, _) => new ProtobufDeserializer<T>());

        return services;
    }
}