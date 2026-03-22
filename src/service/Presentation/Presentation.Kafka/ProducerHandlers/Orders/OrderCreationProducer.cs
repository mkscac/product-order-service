using Google.Protobuf.WellKnownTypes;
using Kafka.Producer;
using Orders.Kafka.Contracts;

namespace Presentation.Kafka.ProducerHandlers.Orders;

public class OrderCreationProducer : IOrderCreationProducer
{
    private readonly IKafkaMessageProducer<OrderCreationKey, OrderCreationValue> _producer;

    public OrderCreationProducer(IKafkaMessageProducer<OrderCreationKey, OrderCreationValue> producer)
    {
        _producer = producer;
    }

    public async Task ProduceOrderCreatedAsync(long orderId, CancellationToken ct)
    {
        var key = new OrderCreationKey { OrderId = orderId };

        var value = new OrderCreationValue
        {
            OrderCreated = new OrderCreationValue.Types.OrderCreated
            {
                OrderId = orderId,
                CreatedAt = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow),
            },
        };

        await _producer.ProduceAsync(key, value, ct);
    }

    public async Task ProduceOrderProcessingStartedAsync(long orderId, CancellationToken ct)
    {
        var key = new OrderCreationKey { OrderId = orderId };

        var value = new OrderCreationValue
        {
            OrderProcessingStarted = new OrderCreationValue.Types.OrderProcessingStarted
            {
                OrderId = orderId,
                StartedAt = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow),
            },
        };

        await _producer.ProduceAsync(key, value, ct);
    }
}