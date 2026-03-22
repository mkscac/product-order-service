using Confluent.Kafka;

namespace Kafka.Consumer.Models;

public class KafkaConsumerMessage<TKey, TValue> : IKafkaConsumerMessage<TKey, TValue>
{
    public TKey Key { get; }

    public TValue Value { get; }

    public string Topic { get; }

    public KafkaConsumerMessage(ConsumeResult<TKey, TValue> consumeResult, string topic)
    {
        Key = consumeResult.Message.Key;
        Value = consumeResult.Message.Value;
        Topic = topic;
    }
}