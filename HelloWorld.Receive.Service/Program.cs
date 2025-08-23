using HelloWorld.Service;

namespace HelloWorld.Receive.Service;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<Service>();

        builder.Configuration.AddInMemoryCollection();
        builder.Services.Configure<RabbitMqConnection>(builder.Configuration.GetSection(RabbitMqConnection.Section));

        var host = builder.Build();
        host.Run();
    }
}