namespace Presentation.Kafka;

public interface IOrderCreationProducer
{
    Task ProduceOrderCreatedAsync(long orderId, CancellationToken ct);

    Task ProduceOrderProcessingStartedAsync(long orderId, CancellationToken ct);
}