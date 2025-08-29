namespace Base.Service.Configurations;

public class RabbitMqConnection : IConfigurationCollection
{
    public const string Section = nameof(RabbitMqConnection);
    
    public required string ConnectionString { get; init; }
    public required string QueueName { get; init; }
    
    public IEnumerable<KeyValuePair<string, string?>> GetConfigurationCollection()
    {
        yield return new KeyValuePair<string, string?>(Section + ':' + nameof(ConnectionString), ConnectionString);
        yield return new KeyValuePair<string, string?>(Section + ':' + nameof(QueueName), QueueName);
    }
}