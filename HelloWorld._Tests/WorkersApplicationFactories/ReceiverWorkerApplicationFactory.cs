using HelloWorld.Receive.Worker;
using HelloWorld.Receive.Worker.MessageStore;
using HelloWorld.Send.Tests.Fakes;
using HelloWorld.Worker;
using HelloWorld.Worker.DelaySource;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HelloWorld.Send.Tests.WorkersApplicationFactories;

public class ReceiverWorkerApplicationFactory : WebApplicationFactory<Program>
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
            collection.AddSingleton<IMessageStore, MessageStoreFake>();
            collection.AddSingleton<IDelaySource, DelaySourceFake>();
        });

        builder.Configure(_ => { });
    }

    public void AddRabbitMqConnection(RabbitMqConnection connection)
    {
        _rabbitMqConnection = connection;
    }
}