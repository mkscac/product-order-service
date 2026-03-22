using Confluent.Kafka;
using Kafka.Common.Options;
using Kafka.Producer.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Kafka.Producer.Services;

public class KafkaMessageProducer<TKey, TValue> : IKafkaMessageProducer<TKey, TValue>, IDisposable
{
    private readonly IProducer<TKey, TValue> _producer;
    private readonly KafkaProducerOptions _producerOptions;

    public KafkaMessageProducer(string producerName, IServiceProvider provider)
    {
        IOptionsMonitor<KafkaProducerOptions> producerOptionsMonitor =
            provider.GetRequiredService<IOptionsMonitor<KafkaProducerOptions>>();
        _producerOptions = producerOptionsMonitor.Get(producerName);

        KafkaPlatformOptions options = provider.GetRequiredService<IOptionsMonitor<KafkaPlatformOptions>>().CurrentValue;
        var config = new ProducerConfig
        {
            BootstrapServers = options.BootstrapServers,
        };

        ISerializer<TKey> keySerializer = provider.GetRequiredKeyedService<ISerializer<TKey>>(producerName);
        ISerializer<TValue> valueSerializer = provider.GetRequiredKeyedService<ISerializer<TValue>>(producerName);

        _producer = new ProducerBuilder<TKey, TValue>(config)
            .SetKeySerializer(keySerializer)
            .SetValueSerializer(valueSerializer)
            .Build();
    }

    public async Task ProduceAsync(TKey key, TValue value, CancellationToken ct)
    {
        var message = new Message<TKey, TValue>
        {
            Key = key,
            Value = value,
        };
        await _producer.ProduceAsync(_producerOptions.Topic, message, ct);
    }

    public void Dispose()
    {
        _producer.Dispose();
    }
}