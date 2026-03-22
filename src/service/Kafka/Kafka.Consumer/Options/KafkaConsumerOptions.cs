namespace Kafka.Consumer.Options;

public class KafkaConsumerOptions
{
    public string Topic { get; set; } = string.Empty;

    public string GroupId { get; set; } = string.Empty;

    public int BatchSize { get; set; } = 1;
}