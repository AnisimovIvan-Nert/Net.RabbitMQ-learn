using _Tests.Fakes;
using Base.Service;
using Base.Service.DelaySource;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WorkQueues.Tests.Fakes;
using WorkQueues.Worker.Service;
using WorkQueues.Worker.Service.CompletedTaskCountStore;

namespace WorkQueues.Tests.ServicesApplicationFactories;

public class WorkerServiceApplicationFactory : WebApplicationFactory<Program>
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
            collection.AddSingleton<ICompletedTaskStore, CompletedTaskStoreFake>();
            collection.AddSingleton<IDelaySource, DelaySourceFake>();
        });

        builder.Configure(_ => { });
    }

    public void AddRabbitMqConnection(RabbitMqConnection connection)
    {
        _rabbitMqConnection = connection;
    }
}