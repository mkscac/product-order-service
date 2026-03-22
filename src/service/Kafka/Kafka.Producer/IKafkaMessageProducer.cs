namespace Kafka.Producer;

public interface IKafkaMessageProducer<TKey, TValue>
{
    Task ProduceAsync(TKey key, TValue value, CancellationToken ct);
}