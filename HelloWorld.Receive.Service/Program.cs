using Base.Service.Configurations;

namespace HelloWorld.Receive.Service;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<Service>();
        builder.Services.AddSystemd();

        builder.Configuration.AddInMemoryCollection();
        builder.Services.Configure<RabbitMqConnection>(builder.Configuration.GetSection(RabbitMqConnection.Section));
        builder.Services.Configure<RabbitMqReceiverAcknowledgment>(builder.Configuration.GetSection(RabbitMqReceiverAcknowledgment.Section));

        var host = builder.Build();
        host.Run();
    }
}