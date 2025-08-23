using HelloWorld.Worker;

namespace HelloWorld.Receive.Worker;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<Worker>();

        builder.Configuration.AddInMemoryCollection();
        builder.Services.Configure<RabbitMqConnection>(builder.Configuration.GetSection(RabbitMqConnection.Section));

        var host = builder.Build();
        host.Run();
    }
}