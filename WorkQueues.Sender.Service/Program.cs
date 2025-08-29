using Base.Service.Configurations;

namespace WorkQueues.Sender.Service;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<Service>();
        builder.Services.AddSystemd();

        RabbitMqOptions.Configure(builder);

        var host = builder.Build();
        host.Run();
    }
}