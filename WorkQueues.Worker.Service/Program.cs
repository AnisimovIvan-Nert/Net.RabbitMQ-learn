using Base.Service.Configurations;

namespace WorkQueues.Worker.Service;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<Service>();
        builder.Services.AddSystemd();

        builder.Configuration.AddInMemoryCollection();
        
        RabbitMqOptions.Configure(builder);

        var host = builder.Build();
        host.Run();
    }
}