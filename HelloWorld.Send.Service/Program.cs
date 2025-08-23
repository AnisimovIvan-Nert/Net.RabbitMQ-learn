using Base.Service;

namespace HelloWorld.Sen.Service;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<Service>();
        builder.Services.AddSystemd();

        builder.Services.Configure<RabbitMqConnection>(builder.Configuration.GetSection(RabbitMqConnection.Section));

        var host = builder.Build();
        host.Run();
    }
}