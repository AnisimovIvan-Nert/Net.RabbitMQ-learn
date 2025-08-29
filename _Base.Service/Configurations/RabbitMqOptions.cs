using Microsoft.Extensions.Hosting;

namespace Base.Service.Configurations;

public class RabbitMqOptions : IConfigurationCollection
{
    public const string Section = nameof(RabbitMqOptions);
    
    public required RabbitMqConnection Connection { get; init; }
    public required RabbitMqReceiverAcknowledgment Acknowledgment { get; init; }

    public static void Configure(HostApplicationBuilder builder, string parentSection = "")
    {
        ConfigurationHelper.Configure<RabbitMqOptions>(builder, Section, parentSection, 
            RabbitMqConnection.Configure, RabbitMqReceiverAcknowledgment.Configure);
    }

    public IEnumerable<KeyValuePair<string, string?>> GetConfigurationCollection()
    {
        return ConfigurationHelper.GetConfigurationPairs(this, Section, 
            Connection, Acknowledgment);
    }
}