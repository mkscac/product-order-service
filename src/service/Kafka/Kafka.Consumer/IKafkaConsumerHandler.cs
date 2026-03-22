namespace Kafka.Consumer;

public interface IKafkaConsumerHandler<TKey, TValue>
{
    Task HandleBatchAsync(IEnumerable<IKafkaConsumerMessage<TKey, TValue>> messages, CancellationToken ct);
}