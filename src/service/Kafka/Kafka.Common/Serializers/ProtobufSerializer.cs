using Confluent.Kafka;
using Google.Protobuf;

namespace Kafka.Common.Serializers;

public sealed class ProtobufSerializer<T> : ISerializer<T> where T : IMessage<T>
{
    public byte[] Serialize(T data, SerializationContext context)
    {
        return data.ToByteArray();
    }
}