using _Tests.Fakes;
using Base.Service;
using Base.Service.DelaySource;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WorkQueues.Sender.Service;
using WorkQueues.Sender.Service.TaskSource;
using WorkQueues.Tests.Fakes;

namespace WorkQueues.Tests.ServicesApplicationFactories;

public class SenderServiceApplicationFactory : WebApplicationFactory<Program>
{
    private RabbitMqConnection? _rabbitMqConnection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        if (_rabbitMqConnection == null)
            throw new InvalidOperationException();

        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddInMemoryCollection(_rabbitMqConnection.GetConfigurationCollection());
        });

        builder.ConfigureServices(collection =>
        {
            collection.AddSingleton<ITaskSource, TaskSourceFake>();
            collection.AddSingleton<IDelaySource, DelaySourceFake>();
        });

        builder.Configure(_ => { });
    }

    public void AddRabbitMqConnection(RabbitMqConnection connection)
    {
        _rabbitMqConnection = connection;
    }
}