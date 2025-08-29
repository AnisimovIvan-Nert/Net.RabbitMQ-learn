using Microsoft.Extensions.Hosting;

namespace Base.Service.Configurations;

public class RabbitMqConnection : IConfigurationCollection
{
    public const string Section = nameof(RabbitMqConnection);
    
    public required string ConnectionString { get; init; }
    public required string QueueName { get; init; }
    public required bool Durable { get; init; }

    public static void Configure(HostApplicationBuilder builder, string parentSection = "")
    {
        ConfigurationHelper.Configure<RabbitMqConnection>(builder, Section, parentSection);
    }

    public IEnumerable<KeyValuePair<string, string?>> GetConfigurationCollection()
    {
        return ConfigurationHelper.GetConfigurationPairs(this, Section);
    }
}