namespace HelloWorld.Worker;

public class RabbitMqConnection : IConfigurationCollection
{
    public const string Section = nameof(RabbitMqConnection);
    
    public string ConnectionString { get; set; }
    public string QueueName { get; set; }
    
    public IEnumerable<KeyValuePair<string, string?>> GetConfigurationCollection()
    {
        yield return new KeyValuePair<string, string?>(Section + ':' + nameof(ConnectionString), ConnectionString);
        yield return new KeyValuePair<string, string?>(Section + ':' + nameof(QueueName), QueueName);
    }
}