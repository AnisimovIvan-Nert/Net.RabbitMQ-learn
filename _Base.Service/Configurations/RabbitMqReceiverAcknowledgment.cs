using Microsoft.Extensions.Hosting;

namespace Base.Service.Configurations;

public class RabbitMqReceiverAcknowledgment : IConfigurationCollection
{
    public const string Section = nameof(RabbitMqReceiverAcknowledgment);

    public required bool Value { get; init; }
    
    public static void Configure(HostApplicationBuilder builder, string parentSection = "")
    {
        ConfigurationHelper.Configure<RabbitMqReceiverAcknowledgment>(builder, Section, parentSection);
    }

    public IEnumerable<KeyValuePair<string, string?>> GetConfigurationCollection()
    {
        return ConfigurationHelper.GetConfigurationPairs(this, Section);
    }
}