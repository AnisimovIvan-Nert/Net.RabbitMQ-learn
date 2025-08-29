namespace Base.Service.Configurations;

public class RabbitMqReceiverAcknowledgment : IConfigurationCollection
{
    public const string Section = nameof(RabbitMqReceiverAcknowledgment);

    public required bool Value { get; init; }
    
    public IEnumerable<KeyValuePair<string, string?>> GetConfigurationCollection()
    {
        yield return new KeyValuePair<string, string?>(Section + ':' + nameof(Value), Value.ToString());
    }
}