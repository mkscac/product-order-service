using Confluent.Kafka;
using Kafka.Common.Options;
using Kafka.Consumer.Models;
using Kafka.Consumer.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Kafka.Consumer.Services;

public class KafkaBackgroundServiceConsumer<TKey, TValue> : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly string _consumerName;

    public KafkaBackgroundServiceConsumer(string consumerName, IServiceScopeFactory scopeFactory)
    {
        _consumerName = consumerName;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        IServiceProvider provider = scope.ServiceProvider;

        KafkaConsumerOptions consumerOptions =
            provider.GetRequiredService<IOptionsMonitor<KafkaConsumerOptions>>().Get(_consumerName);

        KafkaPlatformOptions platformOptions =
            provider.GetRequiredService<IOptionsMonitor<KafkaPlatformOptions>>().CurrentValue;
        var config = new ConsumerConfig
        {
            BootstrapServers = platformOptions.BootstrapServers,
            GroupId = consumerOptions.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
        };

        IDeserializer<TKey> keyDeserializer = provider.GetRequiredKeyedService<IDeserializer<TKey>>(_consumerName);
        IDeserializer<TValue> valueDeserializer = provider.GetRequiredKeyedService<IDeserializer<TValue>>(_consumerName);

        using IConsumer<TKey, TValue> consumer = new ConsumerBuilder<TKey, TValue>(config)
            .SetKeyDeserializer(keyDeserializer)
            .SetValueDeserializer(valueDeserializer)
            .Build();

        consumer.Subscribe(consumerOptions.Topic);

        var batch = new List<IKafkaConsumerMessage<TKey, TValue>>(consumerOptions.BatchSize);

        try
        {
            while (stoppingToken.IsCancellationRequested is false)
            {
                ConsumeResult<TKey, TValue> result = consumer.Consume(stoppingToken);

                var message = new KafkaConsumerMessage<TKey, TValue>(result, consumerOptions.Topic);
                batch.Add(message);
                if (batch.Count < consumerOptions.BatchSize)
                    continue;

                await Handle(batch, stoppingToken);
                batch.Clear();
                consumer.Commit();
            }
        }
        finally
        {
            if (batch.Count > 0)
            {
                await Handle(batch, CancellationToken.None);
                consumer.Commit();
                batch.Clear();
            }

            consumer.Close();
        }
    }

    private async Task Handle(List<IKafkaConsumerMessage<TKey, TValue>> batch, CancellationToken stoppingToken)
    {
        await using AsyncServiceScope batchScope = _scopeFactory.CreateAsyncScope();
        IKafkaConsumerHandler<TKey, TValue> handler =
            batchScope.ServiceProvider.GetRequiredService<IKafkaConsumerHandler<TKey, TValue>>();

        await handler.HandleBatchAsync(batch, stoppingToken);
    }
}