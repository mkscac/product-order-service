namespace Kafka.Consumer;

public interface IKafkaConsumerMessage<TKey, TValue>
{
    TKey Key { get; }

    TValue Value { get; }

    string Topic { get; }
}