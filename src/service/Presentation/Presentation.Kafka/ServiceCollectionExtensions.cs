using Kafka.Common;
using Kafka.Common.Options;
using Kafka.Consumer;
using Kafka.Consumer.Options;
using Kafka.Consumer.Services;
using Kafka.Producer;
using Kafka.Producer.Options;
using Kafka.Producer.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orders.Kafka.Contracts;
using Presentation.Kafka.ConsumerHandlers;
using Presentation.Kafka.ProducerHandlers.Orders;

namespace Presentation.Kafka;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationKafka(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        const string orderCreationProducerName = "OrderCreation";
        const string orderProcessingConsumerName = "OrderProcessing";

        services.Configure<KafkaPlatformOptions>(configuration.GetSection("Presentation:Kafka"));

        services
            .Configure<KafkaProducerOptions>(
                orderCreationProducerName,
                configuration.GetSection($"Presentation:Kafka:Producers:{orderCreationProducerName}"))
            .Configure<KafkaConsumerOptions>(
                orderProcessingConsumerName,
                configuration.GetSection($"Presentation:Kafka:Consumers:{orderProcessingConsumerName}"));

        services
            .AddProtobufSerializerDeserializer<OrderCreationKey>(orderCreationProducerName)
            .AddProtobufSerializerDeserializer<OrderCreationValue>(orderCreationProducerName)
            .AddProtobufSerializerDeserializer<OrderProcessingKey>(orderProcessingConsumerName)
            .AddProtobufSerializerDeserializer<OrderProcessingValue>(orderProcessingConsumerName);

        services
            .AddSingleton<IKafkaMessageProducer<OrderCreationKey, OrderCreationValue>>(sp =>
                new KafkaMessageProducer<OrderCreationKey, OrderCreationValue>(orderCreationProducerName, sp))
            .AddScoped<IOrderCreationProducer, OrderCreationProducer>();

        services
            .AddSingleton<
                IKafkaConsumerHandler<OrderProcessingKey, OrderProcessingValue>,
                OrderProcessingConsumerHandler>()
            .AddHostedService<KafkaBackgroundServiceConsumer<OrderProcessingKey, OrderProcessingValue>>(sp =>
                new(
                    orderProcessingConsumerName,
                    sp.GetRequiredService<IServiceScopeFactory>()));

        return services;
    }
}